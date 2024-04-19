using ReactiveUI;
using RedisViewDesktop.Models;
using System.Reactive;

namespace RedisViewDesktop.ViewModels
{
    public class NewKeyViewModel : ViewModelBase
    {
        private bool isEnabled = false;
        public bool IsEnabled
        {
            get => isEnabled;
            set
            {
                this.RaiseAndSetIfChanged(ref isEnabled, value);
            }
        }

        private string? key;
        public string? Key
        {
            get => key;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    IsEnabled = true;
                }
                else
                {
                    IsEnabled = false;
                }
                this.RaiseAndSetIfChanged(ref key, value);
            }
        }

        private NewKey? selectedItem;
        public NewKey? SelectedItem
        {
            get => selectedItem;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedItem, value);
            }
        }

        public ReactiveCommand<Unit, AddKey> AddComand { get; set; }
        public ReactiveCommand<Unit, AddKey?> CancelCommand { get; set; }
        public NewKeyViewModel()
        {
            AddComand = ReactiveCommand.Create(() =>
            {
                return new AddKey(SelectedItem!, Key!);
            });
            CancelCommand = ReactiveCommand.Create(() =>
            {
                AddKey? add = null;
                return add;
            });
        }
    }
}