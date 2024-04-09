using ReactiveUI;
using System.Reactive;

namespace RedisViewDesktop.ViewModels
{
    public class ZSetMemberEditViewModel : ViewModelBase
    {
        public string? Key { get; set; }
        public string? Member { get; set; }
        private string? score;
        public string? Score
        {
            get => score;
            set
            {
                this.RaiseAndSetIfChanged(ref score, value);
            }
        }

        public ReactiveCommand<Unit, ZSetMemberViewModel?> EditCommand { get; set; }
        public ReactiveCommand<Unit, ZSetMemberViewModel?> CancelCommand { get; }

        public ZSetMemberEditViewModel()
        {
            EditCommand = ReactiveCommand.Create(() =>
            {
                if (double.TryParse(Score, out double d))
                {
                    return new ZSetMemberViewModel(Key!, Member!, d);
                }
                return null;
            });
            CancelCommand = ReactiveCommand.Create(() =>
            {
                ZSetMemberViewModel? zSet = null;
                return zSet;
            });

        }
    }
}