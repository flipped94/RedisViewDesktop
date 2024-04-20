using DynamicData;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows.Input;

namespace RedisViewDesktop.ViewModels
{
    public class ZSetViewModel : KeyViewModelBase
    {

        private ZSetMemberViewModel? selectedMember;
        public ZSetMemberViewModel? SelectedMember
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


        private readonly IEnumerable<ZSetMemberViewModel> EMPTY = [];
        public ObservableCollection<ZSetMemberViewModel> Content { get; } = [];
        public ObservableCollection<ZSetMemberViewModel> OriginContent { get; } = [];

        //public ICommand SearchMembersCommand { get; }
        //public ICommand DeleteCommand { get; }
        public ICommand AddMembersCommand { get; }
        public ICommand EditMemberCommand { get; }
        public ICommand DeleteMemberCommand { get; }

        public Interaction<ZSetMemberViewModel, ZSetMemberViewModel?> ShowEditSortedSetMemberDialog { get; }

        public Interaction<ZSetMemberAddViewModel, Dictionary<string, double>> ShowAddMembersDialog { get; }


        public ZSetViewModel()
        {
            //SearchMembersCommand = ReactiveCommand.Create(() =>
            //{
            //    SearchMembers();
            //});

            //DeleteCommand = ReactiveCommand.Create(async () =>
            //{
            //    var res = await RedisHelper.DeleteKey(Key);
            //    if (res)
            //    {
            //        KeysPage!.CurrentPage = new SelectAKeyViewModel();
            //    }
            //});

            ShowAddMembersDialog = new Interaction<ZSetMemberAddViewModel, Dictionary<string, double>>();
            AddMembersCommand = ReactiveCommand.Create(async () =>
            {
                ZSetMemberAddViewModel store = new();
                Dictionary<string, double> result = await ShowAddMembersDialog.Handle(store);
                if (result != null)
                {
                    var res = RedisHelper.ZSetAddMembersAsync(Key, result);
                    foreach (var item in Content)
                    {
                        if (result.TryGetValue(item.Member, out double value))
                        {
                            item.Score = value;
                            result.Remove(item.Member);
                        }
                    }
                    foreach (var item in result.Select(x => x))
                    {
                        var add = new ZSetMemberViewModel(Key, item.Key, item.Value);
                        Content.Add(add);
                        OriginContent.Add(add);
                    }
                }
            });

            ShowEditSortedSetMemberDialog = new Interaction<ZSetMemberViewModel, ZSetMemberViewModel?>();
            EditMemberCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (SelectedMember != null)
                {
                    var result = await ShowEditSortedSetMemberDialog.Handle(SelectedMember);
                    if (result != null)
                    {
                        await RedisHelper.ZSetUpdateScore(Key, result.Member, result.Score);
                        Content.Where(x => x.Member.Equals(result.Member)).First().Score = result.Score;
                        OriginContent.Where(x => x.Member.Equals(result.Member)).First().Score = result.Score;
                    }
                }
            });

            DeleteMemberCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (SelectedMember != null)
                {
                    var res = await RedisHelper.ZSetDeleteMember(SelectedMember.Key, SelectedMember.Member);
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
                var filter = OriginContent.Where(x => x.Member.Contains(queryString)).Select(x => x);
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
            Dictionary<string, object> info = await RedisHelper.ZSetDetail(Key);
            MemoryUsage = (string)info.GetValueOrDefault("memory", "");
            Length = (string)info.GetValueOrDefault("len", "");
            TTL = (string)info.GetValueOrDefault("ttl", "");
            IEnumerable<ZSetMemberViewModel> members = (IEnumerable<ZSetMemberViewModel>)info.GetValueOrDefault("content", EMPTY);
            OriginContent.AddRange(members);
            Content.AddRange(members);
        }
    }
}