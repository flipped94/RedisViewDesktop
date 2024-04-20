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
    public class HashViewModel : KeyViewModelBase
    {

        private HashFieldViewModel? selectedField;
        public HashFieldViewModel? SelectedField
        {
            get => selectedField;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedField, value);
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


        private readonly IEnumerable<HashFieldViewModel> EMPTY = [];
        public ObservableCollection<HashFieldViewModel> Content { get; } = [];
        public ObservableCollection<HashFieldViewModel> OriginContent { get; } = [];

        //public ICommand SearchMembersCommand { get; }
        //public ICommand DeleteCommand { get; }
        public ICommand AddFieldsCommand { get; }
        public ICommand EditFieldCommand { get; }
        public ICommand DeleteFieldCommand { get; }

        public Interaction<HashFieldAddViewModel, Dictionary<string, string>> ShowAddFieldsDialog { get; }
        public Interaction<HashFieldViewModel, HashFieldViewModel?> ShowEditHashFieldDialog { get; }

        public HashViewModel()
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

            ShowAddFieldsDialog = new Interaction<HashFieldAddViewModel, Dictionary<string, string>>();
            AddFieldsCommand = ReactiveCommand.Create(async () =>
            {

                HashFieldAddViewModel store = new();
                Dictionary<string, string> result = await ShowAddFieldsDialog.Handle(store);
                if (result != null)
                {
                    var res = RedisHelper.HasAddFieldsAsync(Key, result);
                    foreach (var item in Content)
                    {
                        if (result.TryGetValue(item.Field, out var value))
                        {
                            item.Value = value;
                            result.Remove(item.Field);
                        }
                    }

                    foreach (var item in result.Select(x => x))
                    {
                        var add = new HashFieldViewModel(Key, item.Key, item.Value);
                        Content.Add(add);
                        Content.Add(add);
                    }
                }
            });

            ShowEditHashFieldDialog = new Interaction<HashFieldViewModel, HashFieldViewModel?>();
            EditFieldCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (SelectedField != null)
                {
                    var result = await ShowEditHashFieldDialog.Handle(SelectedField);
                    if (result != null)
                    {
                        Dictionary<string, string> field = new()
                        {
                            { SelectedField.Field, result.Value }
                        };
                        Content.Where(x => x.Field.Equals(result.Field)).First().Value = result.Value;
                        OriginContent.Where(x => x.Field.Equals(result.Field)).First().Value = result.Value;
                        await RedisHelper.HasAddFieldsAsync(Key, field);
                    }
                }
            });

            DeleteFieldCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (SelectedField != null)
                {
                    var res = await RedisHelper.HashDeleteField(SelectedField.Key, SelectedField.Field);
                    if (res)
                    {
                        Content.Remove(SelectedField);
                        OriginContent.Remove(SelectedField);
                    }
                }
            });
        }

        private void SearchMembers()
        {
            if (!string.IsNullOrEmpty(queryString))
            {
                var filter = OriginContent.Where(x => x.Field.Contains(queryString)).Select(x => x);
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
            Dictionary<string, object> info = await RedisHelper.HashDetail(Key);
            MemoryUsage = (string)info.GetValueOrDefault("memory", "");
            Length = (string)info.GetValueOrDefault("len", "");
            TTL = (string)info.GetValueOrDefault("ttl", "");
            IEnumerable<HashFieldViewModel> fields = (IEnumerable<HashFieldViewModel>)info.GetValueOrDefault("content", EMPTY);
            OriginContent.AddRange(fields);
            Content.AddRange(fields);
        }
    }
}