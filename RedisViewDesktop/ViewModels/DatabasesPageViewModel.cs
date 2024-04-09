using DynamicData;
using ReactiveUI;
using RedisViewDesktop.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Linq;

namespace RedisViewDesktop.ViewModels
{
    public class DatabasesPageViewModel : ViewModelBase
    {
        private bool isHasDatabases;
        public bool IsHasDatabases
        {
            get => isHasDatabases;
            set
            {
                this.RaiseAndSetIfChanged(ref isHasDatabases, value);
            }
        }

        public ObservableCollection<ConnectionViewModel> Connections { get; } = [];

        public ReactiveCommand<Unit, Unit> ShowAddConnectionDialgCommand { get; }

        public Interaction<ConnectionStoreViewModel, ConnectionViewModel?> AddConnectionInteraction { get; }

        public DatabasesPageViewModel()
        {
            RxApp.MainThreadScheduler.Schedule(LoadConnections);

            AddConnectionInteraction = new Interaction<ConnectionStoreViewModel, ConnectionViewModel?>();
            ShowAddConnectionDialgCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                var store = new ConnectionStoreViewModel();
                var result = await AddConnectionInteraction.Handle(store);
                if (result is not null)
                {
                    Connections.Add(result);
                    IsHasDatabases = true;
                    await result.SaveToDB();
                }
            });

        }

        private async void LoadConnections()
        {
            var connections = (await Connection.LoadAsync()).Select(x => new ConnectionViewModel(x));
            if (connections.Any())
            {
                IsHasDatabases = true;
            }
            else
            {
                IsHasDatabases = false;
            }
            foreach (var item in connections)
            {
                Connections.Add(item);
            }
        }

        public async void Delete(int id)
        {
            await Connection.Delete(id);
            var deletes = Connections.Where(x => x.Id == id).ToList();
            if (deletes!.Count > 0)
            {
                Connections.Remove(deletes);
                if (Connections.Count == 0)
                {
                    IsHasDatabases = false;
                }
            }
        }
    }
}
