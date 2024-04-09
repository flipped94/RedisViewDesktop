using Avalonia.Controls;
using Avalonia.ReactiveUI;
using ReactiveUI;
using RedisViewDesktop.Helpers;
using RedisViewDesktop.ViewModels;
using System.Threading.Tasks;

namespace RedisViewDesktop.Views;

public partial class DatabasesPageView : ReactiveUserControl<DatabasesPageViewModel>
{
    public DatabasesPageView()
    {
        InitializeComponent();
        this.WhenActivated(action => action(ViewModel!.AddConnectionInteraction.RegisterHandler(DoShowAddConnectionDialog)));
        AddHandler(PointerPressedEvent, Action);
    }

    private async Task DoShowAddConnectionDialog(InteractionContext<ConnectionStoreViewModel, ConnectionViewModel?> interaction)
    {
        var dialog = new ConnectionWindow
        {
            DataContext = interaction.Input
        };
        var connection = await dialog.ShowDialog<ConnectionViewModel?>(WindowHelper.FindParentWindow(this)!);
        interaction.SetOutput(connection);
    }

    private void Action(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Control c = (Control)e.Source!;
        if (c.Name != null && c.Name.StartsWith("DeleteConnection"))
        {
            ConnectionViewModel viewModel = (ConnectionViewModel)c.DataContext!;
            ViewModel!.Delete(viewModel!.Id);
            e.Handled = true;
            return;
        }
        else if (c.Name != null && c.Name.StartsWith("OpenConnection"))
        {            
            e.Handled = false;
            return;
        }
        else if (c.Name != null && c.Name.StartsWith("DetailConnection"))
        {
            e.Handled = false;
            return;
        }
        e.Handled = true;
    }

}
