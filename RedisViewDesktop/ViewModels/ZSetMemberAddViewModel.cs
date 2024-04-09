using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace RedisViewDesktop.ViewModels
{
    public class ZSetMember : ViewModelBase
    {
        public static int Total = 1;

        public int Index { get; set; }
        private string member;
        public string Member
        {
            get => member;
            set
            {

                this.RaiseAndSetIfChanged(ref member, value);
                DoShow(Member, Score);
            }
        }
        private string score;
        public string Score
        {
            get => score;
            set
            {
                this.RaiseAndSetIfChanged(ref score, value);
                DoShow(Member, Score);
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


        public ZSetMember(string member, string score, bool showAdd, bool showDel, int index)
        {
            Index = index;
            Member = member;
            Score = score;
            ShowAdd = showAdd;
            ShowDel = showDel;
        }

        public void DoShow(string member, string score)
        {
            // only one row
            if (Index == 1 && Total == 1)
            {
                if (string.IsNullOrEmpty(member) && !double.TryParse(score, out _))
                {
                    ShowDel = false;
                    ShowAdd = false;
                    return;
                }

                else if (string.IsNullOrEmpty(member) && double.TryParse(Score, out _))
                {
                    ShowDel = true;
                    ShowAdd = false;
                    return;
                }
                else if (!string.IsNullOrEmpty(member) && !double.TryParse(Score, out _))
                {
                    ShowDel = true;
                    ShowAdd = false;
                    return;
                }
                else if (!string.IsNullOrEmpty(member) && double.TryParse(Score, out _))
                {
                    ShowDel = true;
                    ShowAdd = true;
                    return;
                }
                return;
            }
            // first row but more than one
            if (Index == 1 && Total > 1)
            {
                if (string.IsNullOrEmpty(member) && !double.TryParse(score, out _))
                {
                    ShowDel = true;
                    ShowAdd = false;
                    return;
                }

                else if (string.IsNullOrEmpty(member) && double.TryParse(Score, out _))
                {
                    ShowDel = true;
                    ShowAdd = false;
                    return;
                }
                else if (!string.IsNullOrEmpty(member) && !double.TryParse(Score, out _))
                {
                    ShowDel = true;
                    ShowAdd = false;
                    return;
                }
                else if (!string.IsNullOrEmpty(member) && double.TryParse(Score, out _))
                {
                    ShowDel = true;
                    ShowAdd = false;
                    return;
                }
                return;
            }
            // more than one and last row
            if (Index > 1 && Index == Total)
            {
                if (!string.IsNullOrEmpty(member) && double.TryParse(Score, out _))
                {
                    ShowAdd = true;
                }
                else
                {
                    ShowAdd = false;
                }
                ShowDel = true;
                return;
            }
            // others
            ShowDel = true;
            ShowAdd = false;
            return;
        }
    }

    public class ZSetMemberAddViewModel : ViewModelBase
    {
        private ZSetMember? selectItem;
        public ZSetMember? SelectedItem
        {
            get => selectItem; set
            {
                this.RaiseAndSetIfChanged(ref selectItem, value);
            }
        }

        public ObservableCollection<ZSetMember> Members { get; } = new ObservableCollection<ZSetMember>() { new ZSetMember("", "", false, false, 1) };

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ReactiveCommand<Unit, Dictionary<string, double>> CancelCommand { get; }
        public ReactiveCommand<Unit, Dictionary<string, double>> SaveCommand { get; }

        public ZSetMemberAddViewModel()
        {

            AddCommand = ReactiveCommand.Create(() =>
            {
                foreach (var item in Members)
                {
                    item.ShowAdd = false;
                    item.ShowDel = true;
                }
                ZSetMember.Total++;
                var add = new ZSetMember("", "", false, true, ZSetMember.Total);
                Members.Add(add);
                SelectedItem = add;
            });

            DeleteCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedItem != null)
                {
                    Members.Remove(SelectedItem);
                    ZSetMember.Total--;
                }
                if (Members.Count == 0)
                {
                    Members.Add(new ZSetMember("", "", false, false, 1));
                    ZSetMember.Total++;
                }
                for (int i = 0; i < Members.Count; i++)
                {
                    var m = Members[i];
                    m.Index = i + 1;
                    m.DoShow(m.Member, m.Score);
                }
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                Dictionary<string, double> res = new();
                foreach (var item in Members)
                {
                    if (string.IsNullOrEmpty(item.Member) || string.IsNullOrEmpty(item.Score))
                    {
                        continue;
                    }
                    if (double.TryParse(item.Score, out double s))
                    {
                        res.TryAdd(item.Member, s);
                    }
                }
                ZSetMember.Total = 1;
                return res;
            });

            CancelCommand = ReactiveCommand.Create(() =>
            {
                Dictionary<string, double> res = [];
                ZSetMember.Total = 1;
                return res;
            });
        }
    }
}