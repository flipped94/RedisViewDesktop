using DynamicData;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace RedisViewDesktop.ViewModels
{

    public class SetViewModel : KeyViewModelBase
    {

        private string? selectedMember;
        public string? SelectedMember
        {
            get => selectedMember;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedMember, value);
            }
        }

        private string queryString = "";
        public string QueryString
        {
            get => queryString;
            set
            {
                this.RaiseAndSetIfChanged(ref queryString, value);
                SearchMembers();
            }
        }


        private readonly HashSet<string> EMPTY = [];
        public ObservableCollection<string> Content { get; } = [];
        public ObservableCollection<string> OriginContent { get; } = [];

        public ICommand DeleteCommand { get; }
        public ICommand AddMembersCommand { get; }
        public ICommand EditMemberCommand { get; }
        public ICommand DeleteMemberCommand { get; }

        public Interaction<Unit, HashSet<string>> ShowAddSetMemberDialog { get; }
        public Interaction<string?, string?> ShowEditSetMemberDialog { get; }

        public SetViewModel()
        {

            DeleteCommand = ReactiveCommand.Create(async () =>
            {
                var res = await RedisHelper.DeleteKey(Key);
                if (res)
                {
                    KeysPage!.CurrentPage = new SelectAKeyViewModel();
                }
            });

            ShowAddSetMemberDialog = new Interaction<Unit, HashSet<string>>();
            AddMembersCommand = ReactiveCommand.Create(async () =>
            {
                HashSet<string> result = await ShowAddSetMemberDialog.Handle(Unit.Default);
                if (result != null && result.Count != 0)
                {
                    var res = RedisHelper.SetAddMembersAsync(Key, result);
                    for (var i = 0; i < Content.Count; i++)
                    {
                        if (result.TryGetValue(Content[i], out var v))
                        {
                            Content[i] = v;
                            result.Remove(v);
                        }
                    }
                    foreach (var item in result.Select(x => x))
                    {
                        Content.Add(item);
                        OriginContent.Add(item);
                    }
                }
            });

            ShowEditSetMemberDialog = new Interaction<string?, string?>();
            EditMemberCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (SelectedMember != null)
                {
                    var result = await ShowEditSetMemberDialog.Handle(SelectedMember);
                    await RedisHelper.SetEditAsync(Key, SelectedMember, result);
                    Content.Remove(SelectedMember);
                    OriginContent.Remove(SelectedMember);
                    if (!Content.Where(x => x.Equals(result)).Any())
                    {
                        Content.Add(result);
                        OriginContent.Add(result);
                    }
                }
            });            

            DeleteMemberCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (SelectedMember != null)
                {
                    var res = await RedisHelper.SetRemoveAsync(Key, SelectedMember);
                    if (res)
                    {
                        Content.Remove(SelectedMember);
                        OriginContent.Remove(SelectedMember);
                    }
                }
            });
        }

        private void SearchMembers()
        {
            if (!string.IsNullOrEmpty(queryString))
            {
                var filter = OriginContent.Where(x => x.Contains(queryString)).Select(x => x);
                Content.Clear();
                Content.AddRange(filter);
            }
            else
            {
                Content.Clear();
                Content.AddRange(OriginContent);
            }
        }

        public async void Ready()
        {
            Dictionary<string, object> info = await RedisHelper.SetDetail(Key);
            MemoryUsage = (string)info.GetValueOrDefault("memory", "");
            Length = (string)info.GetValueOrDefault("len", "");
            TTL = (string)info.GetValueOrDefault("ttl", "");
            HashSet<string> members = (HashSet<string>)info.GetValueOrDefault("content", EMPTY);
            OriginContent.AddRange(members);
            Content.AddRange(members);
        }
    }
}