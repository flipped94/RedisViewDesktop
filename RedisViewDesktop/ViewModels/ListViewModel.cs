using DynamicData;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace RedisViewDesktop.ViewModels
{
    public class ListViewModel : KeyViewModelBase
    {

        private ListNode? selectedElement;
        public ListNode? SelectedElement
        {
            get => selectedElement;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedElement, value);
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


        private readonly IEnumerable<ListNode> EMPTY = [];
        public ObservableCollection<ListNode> Content { get; } = [];
        public ObservableCollection<ListNode> OriginContent { get; } = [];

        //public ICommand DeleteCommand { get; }
        public ICommand AddElementsCommand { get; }
        public ICommand EditElementsCommand { get; }
        public ICommand DeleteElementCommand { get; }

        public Interaction<string, string?> ShowEditListElementDialog { get; }

        public Interaction<Unit, List<ListNode>> ShowAddListElementsDialog { get; }


        public ListViewModel()
        {

            //DeleteCommand = ReactiveCommand.Create(async () =>
            //{
            //    var res = await RedisHelper.DeleteKey(Key);
            //    if (res)
            //    {
            //        KeysPage!.CurrentPage = new SelectAKeyViewModel();
            //    }
            //});

            ShowAddListElementsDialog = new Interaction<Unit, List<ListNode>>();
            AddElementsCommand = ReactiveCommand.Create(async () =>
            {
                List<ListNode> result = await ShowAddListElementsDialog.Handle(Unit.Default);
                if (result != null)
                {
                    var data = result.Select(x => x.Data).ToArray();
                    var res = RedisHelper.ListRpushAcync(Key, data!);
                    Content.AddRange(result);
                    OriginContent.Add(result);
                }
            });

            ShowEditListElementDialog = new Interaction<string, string?>();
            EditElementsCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (SelectedElement != null)
                {
                    var result = await ShowEditListElementDialog.Handle(SelectedElement.Data);
                    {
                        await RedisHelper.ListEditAsync(Key, SelectedElement.Data, result);
                        Content.Where(x => x.UID.Equals(SelectedElement.UID)).First().Data = result;
                        OriginContent.Where(x => x.UID.Equals(SelectedElement.UID)).First().Data = result;
                    }
                }
            });

            DeleteElementCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                if (SelectedElement != null)
                {
                    var res = await RedisHelper.ListRemoveLAsync(Key, SelectedElement.Data);
                    if (res > 0)
                    {
                        Content.Remove(SelectedElement);
                        OriginContent.Remove(SelectedElement);
                    }
                }
            });
        }

        private void SearchMembers()
        {
            if (!string.IsNullOrEmpty(queryString))
            {
                var filter = OriginContent.Where(x => x.Data.Contains(queryString)).Select(x => x);
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
            Dictionary<string, object> info = await RedisHelper.ListDetail(Key);
            MemoryUsage = (string)info.GetValueOrDefault("memory", "");
            Length = (string)info.GetValueOrDefault("len", "");
            TTL = (string)info.GetValueOrDefault("ttl", "");
            IEnumerable<string> elements = (IEnumerable<string>)info.GetValueOrDefault("content", EMPTY);
            IEnumerable<ListNode> nodes = elements.Select(x => new ListNode(Guid.NewGuid().ToString(), x));
            OriginContent.AddRange(nodes);
            Content.AddRange(nodes);
        }
    }
}