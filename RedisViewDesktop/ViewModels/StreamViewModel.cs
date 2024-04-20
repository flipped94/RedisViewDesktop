using DynamicData;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;

namespace RedisViewDesktop.ViewModels
{
    public class StreamViewModel : KeyViewModelBase
    {

        private StreamEntry? selectedEntry;
        public StreamEntry? SelectedEntry
        {
            get => selectedEntry;
            set
            {
                this.RaiseAndSetIfChanged(ref selectedEntry, value);
            }
        }


        private readonly IEnumerable<StreamEntry> EMPTY = [];
        public ObservableCollection<StreamEntry> Content { get; } = [];

        public ICommand ViewEnrtyCommand { get; }
        public ICommand AddEntriesCommand { get; }
        public ICommand DeleteEntryCommand { get; }

        public Interaction<Unit, List<StreamEntry>> ShowAddStreamEntriesDialog { get; }
        public Interaction<StreamEntry, Unit> ShowViewStreamEntrieDialog { get; }


        public StreamViewModel()
        {

            ShowViewStreamEntrieDialog = new Interaction<StreamEntry, Unit>();
            ViewEnrtyCommand = ReactiveCommand.Create(async () =>
            {
                if (SelectedEntry != null)
                {
                    var x = await ShowViewStreamEntrieDialog.Handle(SelectedEntry);
                }

            });

            ShowAddStreamEntriesDialog = new Interaction<Unit, List<StreamEntry>>();

            // TODO
            AddEntriesCommand = ReactiveCommand.Create(() =>
            {
                //List<ListNode> result = await ShowAddListElementsDialog.Handle(Unit.Default);
                //if (result != null)
                //{
                //    var data = result.Select(x => x.Data).ToArray();
                //    var res = RedisHelper.ListRpushAcync(Key, data!);
                //    Content.AddRange(result);
                //    OriginContent.Add(result);
                //}
            });


            DeleteEntryCommand = ReactiveCommand.CreateFromTask(async () =>
            {
                //if (SelectedElement != null)
                //{
                //    var res = await RedisHelper.ListRemoveLAsync(Key, SelectedElement.Data);
                //    if (res > 0)
                //    {
                //        Content.Remove(SelectedElement);
                //        OriginContent.Remove(SelectedElement);
                //    }
                //}
            });
        }

        public async void Ready()
        {
            Dictionary<string, object> info = await RedisHelper.StreamDetail(Key);
            MemoryUsage = (string)info.GetValueOrDefault("memory", "");
            Length = (string)info.GetValueOrDefault("len", "");
            TTL = (string)info.GetValueOrDefault("ttl", "");
            IEnumerable<StreamEntry> entries = (IEnumerable<StreamEntry>)info.GetValueOrDefault("content", EMPTY);
            Content.AddRange(entries);
        }
    }
}