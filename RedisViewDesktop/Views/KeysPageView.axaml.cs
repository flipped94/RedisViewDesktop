using Avalonia.ReactiveUI;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.Models;
using RedisViewDesktop.ViewModels;
using System.Threading.Tasks;

namespace RedisViewDesktop.Views;

public partial class KeysPageView : ReactiveUserControl<KeysPageViewModel>
{
    public KeysPageView()
    {
        InitializeComponent();
        this.WhenActivated(action => action(ViewModel!.AddNewKeyInteraction.RegisterHandler(DoShowAddNewKeyDialog)));
    }

    private async Task DoShowAddNewKeyDialog(InteractionContext<NewKeyViewModel, AddKey?> interaction)
    {
        NewKeyWindow dialog = new NewKeyWindow();
        dialog.DataContext = interaction.Input;
        var newKey = await dialog.ShowDialog<AddKey?>(WindowHelper.FindParentWindow(this)!);
        interaction.SetOutput(newKey);
    }

    private void ScannedKeys_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        KeysScrollViewer.ScrollToEnd();
    }
}