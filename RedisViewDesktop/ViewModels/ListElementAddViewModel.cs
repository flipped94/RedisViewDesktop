using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using ReactiveUI;
using RedisViewDesktop.Models;

namespace RedisViewDesktop.ViewModels
{
    public class Node : ViewModelBase
    {
        public static int Total = 1;

        public int Index { get; set; }
        private string element;
        public string Element
        {
            get => element;
            set
            {
                this.RaiseAndSetIfChanged(ref element, value);
            }
        }

        public bool showAdd;
        public bool ShowAdd
        {
            get => showAdd;
            set
            {
                this.RaiseAndSetIfChanged(ref showAdd, value);
            }
        }

        public bool showDel;
        public bool ShowDel
        {
            get => showDel;
            set
            {
                this.RaiseAndSetIfChanged(ref showDel, value);
            }
        }

        public Node(string element, int index)
        {
            Element = element;
            Index = index;
        }

        public void DoShow()
        {
            if (Index == Total)
            {
                ShowAdd = true;
                ShowDel = true;

            }
            else
            {
                ShowAdd = false;
                ShowDel = true;
            }

        }
    }

    public class ListElementAddViewModel : ViewModelBase
    {
        private Node? selectItem;
        public Node? SelectedItem
        {
            get => selectItem; set
            {
                this.RaiseAndSetIfChanged(ref selectItem, value);
            }
        }

        public ObservableCollection<Node> Elements { get; } = [];

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public ReactiveCommand<Unit, List<ListNode>> CancelCommand { get; }
        public ReactiveCommand<Unit, List<ListNode>> SaveCommand { get; }

        public ListElementAddViewModel()
        {
            Node node = new("", 1);
            node.ShowAdd = true;
            Elements.Add(node);
            AddCommand = ReactiveCommand.Create(() =>
            {
                Node.Total++;
                var add = new Node("", Node.Total);
                Elements.Add(add);
                foreach (var item in Elements)
                {
                    item.DoShow();
                }
                SelectedItem = add;
            });

            DeleteCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedItem != null)
                {
                    Elements.Remove(SelectedItem);
                    Node.Total--;
                }
                if (Elements.Count == 0)
                {
                    Elements.Add(new Node("", 1));
                    Node.Total++;
                }
                for (int i = 0; i < Elements.Count; i++)
                {
                    var m = Elements[i];
                    m.Index = i + 1;
                    m.DoShow();
                }
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                List<ListNode> res = [.. Elements.Select(x => new ListNode(Guid.NewGuid().ToString(), x.Element))];
                Node.Total = 1;
                return res;
            });

            CancelCommand = ReactiveCommand.Create(() =>
            {
                List<ListNode> res = [];
                Node.Total = 1;
                return res;
            });
        }

    }
}