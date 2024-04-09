using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace RedisViewDesktop.ViewModels
{
    public class HashField : ViewModelBase
    {
        public static int Total = 1;

        public int Index { get; set; }
        private string field;
        public string Field
        {
            get => field;
            set
            {

                this.RaiseAndSetIfChanged(ref field, value);
                DoShow();
            }
        }
        private string _value;
        public string Value
        {
            get => _value;
            set
            {
                this.RaiseAndSetIfChanged(ref _value, value);
                DoShow();
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


        public HashField(string field, string value, bool showAdd, bool showDel, int index)
        {
            Index = index;
            Field = field;
            Value = value;
            ShowAdd = showAdd;
            ShowDel = showDel;
        }

        public void DoShow()
        {
            // only one row
            if (Index == 1 && Total == 1)
            {
                if (!string.IsNullOrEmpty(Field) || !string.IsNullOrEmpty(Value))
                {
                    ShowAdd = true;
                    ShowDel = true;
                    return;
                }
                ShowAdd = true;
                ShowDel = false;
                return;
            }
            // first row but more than one
            if (Index == 1 && Total > 1)
            {
                ShowDel = true;
                ShowAdd = false;
                return;
            }
            // more than one and last row
            if (Index > 1 && Index == Total)
            {
                ShowAdd = true;
                ShowDel = true;
                return;
            }
            // others
            ShowDel = true;
            ShowAdd = false;
            return;
        }
    }

    public class HashFieldAddViewModel : ViewModelBase
    {
        private HashField? selectedItem;
        public HashField? SelectedItem
        {
            get => selectedItem; set
            {
                this.RaiseAndSetIfChanged(ref selectedItem, value);
            }
        }

        public ObservableCollection<HashField> Fields { get; } = new ObservableCollection<HashField>() { new HashField("", "", true, false, 1) };

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ReactiveCommand<Unit, Dictionary<string, string>> CancelCommand { get; }
        public ReactiveCommand<Unit, Dictionary<string, string>> SaveCommand { get; }

        public HashFieldAddViewModel()
        {

            AddCommand = ReactiveCommand.Create(() =>
            {
                foreach (var item in Fields)
                {
                    item.ShowAdd = false;
                    item.ShowDel = true;
                }
                HashField.Total++;
                var add = new HashField("", "", true, true, HashField.Total);
                Fields.Add(add);
                SelectedItem = add;
            });

            DeleteCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedItem != null)
                {
                    Fields.Remove(SelectedItem);
                    HashField.Total--;
                }
                if (Fields.Count == 0)
                {
                    Fields.Add(new HashField("", "", false, false, 1));
                    HashField.Total++;
                }
                for (int i = 0; i < Fields.Count; i++)
                {
                    var m = Fields[i];
                    m.Index = i + 1;
                    m.DoShow();
                }
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                Dictionary<string, string> res = new();
                foreach (var item in Fields)
                {
                    res.TryAdd(item.Field, item.Value);
                }
                HashField.Total = 1;
                return res;
            });

            CancelCommand = ReactiveCommand.Create(() =>
            {
                Dictionary<string, string> res = [];
                HashField.Total = 1;
                return res;
            });
        }
    }
}