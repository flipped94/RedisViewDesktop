using ReactiveUI;
using System.Reactive;

namespace RedisViewDesktop.ViewModels
{
    public class HashFieldEditViewModel : ViewModelBase
    {
        public string? Key { get; set; }
        public string? Field { get; set; }
        private string? _value;
        public string? Value
        {
            get => _value;
            set
            {
                this.RaiseAndSetIfChanged(ref _value, value);
            }
        }

        public ReactiveCommand<Unit, HashFieldViewModel> EditCommand { get; set; }
        public ReactiveCommand<Unit, HashFieldViewModel?> CancelCommand { get; }

        public HashFieldEditViewModel()
        {
            EditCommand = ReactiveCommand.Create(() =>
            {
                return new HashFieldViewModel(Key!, Field!, Value);
            });
            CancelCommand = ReactiveCommand.Create(() =>
            {
                HashFieldViewModel? hash = null;
                return hash;
            });

        }
    }
}