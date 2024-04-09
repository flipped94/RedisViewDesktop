using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;
using RedisViewDesktop.ViewModels;
using System.Collections.Generic;
using System.Reactive;
using System.Threading.Tasks;

namespace RedisViewDesktop.Views;

public partial class StreamView : ReactiveUserControl<StreamViewModel>
{
    public StreamView()
    {
        InitializeComponent();
        this.WhenActivated((x) =>
        {
            StreamViewModel streamViewModel = (StreamViewModel)this.DataContext;
            streamViewModel.Ready();
            ViewModel!.ShowViewStreamEntrieDialog.RegisterHandler(DoShowStreamEntryViewDialogAsync);
            ViewModel!.ShowAddStreamEntriesDialog.RegisterHandler(DoShowAddElementsDialogAsync);
        });
    }

    private async Task DoShowAddElementsDialogAsync(InteractionContext<Unit, List<StreamEntry>> interaction)
    {
        StreamEntryAddWindow dialog = new StreamEntryAddWindow();
        Window? parent = WindowHelper.FindParentWindow(this);
        dialog.DataContext = new StreamEntryAddViewModel();
        if (parent != null)
        {
            var result = await dialog.ShowDialog<List<StreamEntry>>(parent);
            if (result != null)
            {
                interaction.SetOutput(result);
            }
        }
    }

    private async Task DoShowStreamEntryViewDialogAsync(InteractionContext<StreamEntry, Unit> interaction)
    {
        StreamEntryViewWindow dialog = new();

        StreamEntry data = interaction.Input;
        StreamEntryViewViewModel dataContext = new(data.EntryID, JsonHelper.Format(data.Value));
        dialog.DataContext = dataContext;
        Window? parent = WindowHelper.FindParentWindow(this);
        if (parent != null)
        {
            await dialog.ShowDialog<string>(parent);
        }
    }
}