using ReactiveUI;
using System.Reactive;

namespace RedisViewDesktop.ViewModels
{
    public class ListElementEditViewModel : ViewModelBase
    {
        private string? elemement;
        public string? Element
        {
            get => elemement;
            set
            {
                this.RaiseAndSetIfChanged(ref elemement, value);
            }
        }

        public ReactiveCommand<Unit, string?> EditCommand { get; set; }
        public ReactiveCommand<Unit, string?> CancelCommand { get; }

        public ListElementEditViewModel()
        {
            EditCommand = ReactiveCommand.Create(() =>
            {
                return Element;
            });
            CancelCommand = ReactiveCommand.Create(() =>
            {
                return Element;
            });

        }
    }
}