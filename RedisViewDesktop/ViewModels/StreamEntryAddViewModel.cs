using ReactiveUI;
using RedisViewDesktop.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;


namespace RedisViewDesktop.ViewModels
{
    public class Entry : ViewModelBase
    {
        public static int Total = 1;

        public int Index { get; set; }

        private string entryId;
        public string EntryID
        {
            get => entryId;
            set
            {
                this.RaiseAndSetIfChanged(ref entryId, value);
            }
        }

        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                this.RaiseAndSetIfChanged(ref _value, value);
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

        public Entry(string entryId, string value, int index)
        {
            EntryID = entryId;
            Value = value;
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

    public class StreamEntryAddViewModel : ViewModelBase
    {
        private Entry? selectItem;
        public Entry? SelectedItem
        {
            get => selectItem; set
            {
                this.RaiseAndSetIfChanged(ref selectItem, value);
            }
        }

        public ObservableCollection<Entry> Elements { get; } = [];

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }

        public ReactiveCommand<Unit, List<StreamEntry>> CancelCommand { get; }
        public ReactiveCommand<Unit, List<StreamEntry>> SaveCommand { get; }

        public StreamEntryAddViewModel()
        {
            Entry node = new("*", "", 1);
            node.ShowAdd = true;
            Elements.Add(node);
            AddCommand = ReactiveCommand.Create(() =>
            {
                Entry.Total++;
                var add = new Entry("*", "", Entry.Total);
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
                    Entry.Total--;
                }
                if (Elements.Count == 0)
                {
                    Elements.Add(new Entry("*", "", 1));
                    Entry.Total++;
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
                List<StreamEntry> res = Elements.Select(x => new StreamEntry(x.EntryID, x.Value)).ToList();
                Entry.Total = 1;
                return res;
            });

            CancelCommand = ReactiveCommand.Create(() =>
            {
                List<StreamEntry> res = [];
                Entry.Total = 1;
                return res;
            });
        }
    }
}