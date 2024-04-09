using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Windows.Input;
using ReactiveUI;

namespace RedisViewDesktop.ViewModels
{
    public class SetMember : ReactiveObject
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


        public SetMember(string member, int index)
        {
            Index = index;
            Member = member;
        }

        public void DoShow()
        {
            if (Index == Total)
            {
                ShowDel = true;
                ShowAdd = true;
            }
            else
            {
                ShowDel = true;
                ShowAdd = false;
            }
        }
    }

    public class SetMemberAddViewModel : ViewModelBase
    {
        private SetMember? selectedMember;
        public SetMember? SelectedMember
        {
            get => selectedMember; set
            {
                this.RaiseAndSetIfChanged(ref selectedMember, value);
            }
        }

        public ObservableCollection<SetMember> Members { get; } = [];

        public ICommand AddCommand { get; }
        public ICommand DeleteCommand { get; }
        public ReactiveCommand<Unit, HashSet<string>> CancelCommand { get; }
        public ReactiveCommand<Unit, HashSet<string>> SaveCommand { get; }

        public SetMemberAddViewModel()
        {
            SetMember member = new SetMember("", 1);
            member.ShowAdd = true;
            member.ShowDel = true;
            Members.Add(member);
            AddCommand = ReactiveCommand.Create(() =>
            {
                SetMember.Total++;
                var add = new SetMember("", SetMember.Total);
                Members.Add(add);
                foreach (var item in Members)
                {
                    item.DoShow();
                }
                SelectedMember = add;
            });

            DeleteCommand = ReactiveCommand.Create(() =>
            {
                if (SelectedMember != null)
                {
                    Members.Remove(SelectedMember);
                    SetMember.Total--;
                }
                if (Members.Count == 0)
                {
                    Members.Add(new SetMember("", 1));
                    SetMember.Total++;
                }
                for (int i = 0; i < Members.Count; i++)
                {
                    var m = Members[i];
                    m.Index = i + 1;
                    m.DoShow();
                }
            });

            SaveCommand = ReactiveCommand.Create(() =>
            {
                var res = Members.Select(x => x.Member).ToHashSet();
                SetMember.Total = 1;
                return res;
            });

            CancelCommand = ReactiveCommand.Create(() =>
            {
                HashSet<string> res = [];
                ZSetMember.Total = 1;
                return res;
            });
        }
    }
}