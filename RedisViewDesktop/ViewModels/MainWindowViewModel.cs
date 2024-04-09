using Avalonia.Media.Imaging;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;

namespace RedisViewDesktop.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private bool isPanleOpen = false;
        public bool IsPanleOpen
        {
            get => isPanleOpen;
            set
            {
                this.RaiseAndSetIfChanged(ref isPanleOpen, value);
            }
        }

        public ReactiveCommand<Unit, Unit> OpenClosePanelCommand { get; }


        private ViewModelBase currentPage = new DatabasesPageViewModel();
        public ViewModelBase CurrentPage
        {
            get => currentPage;
            set
            {
                this.RaiseAndSetIfChanged(ref currentPage, value);
            }
        }

        private ItemTemplate? selectedItem;
        public ItemTemplate? SelectedItem
        {
            get => selectedItem;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedItem, value);
                if (value is null)
                {
                    return;
                }
                var instance = Activator.CreateInstance(value.ModelType);
                if (instance is null)
                {
                    return;
                }
                CurrentPage = (ViewModelBase)instance;
            }
        }

        public ObservableCollection<ItemTemplate> Items { get; } =
        [
            new ItemTemplate(typeof(DatabasesPageViewModel), "avares://RedisViewDesktop/Assets/16x16.png"),
        ];



        public MainWindowViewModel()
        {

            OpenClosePanelCommand = ReactiveCommand.Create(() =>
                        {
                            IsPanleOpen = !IsPanleOpen;
                        });
            this.SelectedItem = Items.First();

        }

    }

    public class ItemTemplate(Type modelType, string itemIcon)
    {
        public string Label { get; } = modelType.Name.Replace("PageViewModel", "");
        public Type ModelType { get; } = modelType;
        public Bitmap ItemIcon { get; } = ImageHelper.LoadFromResources(new Uri(itemIcon));
    }

}
