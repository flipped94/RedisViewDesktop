using Avalonia.Controls;
using Avalonia.Input;
using RedisViewDesktop.ViewModels;

namespace RedisViewDesktop.Views;

public partial class ConnectionView : UserControl
{

    public ConnectionView()
    {
        InitializeComponent();
        DeleteConnectionBtn.PointerPressed += DeleteAction;
        DeleteConnectionIcon.PointerPressed += DeleteAction;
        DeleteConnectionText.PointerPressed += DeleteAction;

        OpenConnectionBtn.PointerPressed += OpenAction;
        OpenConnectionIcon.PointerPressed += OpenAction;
        OpenConnectionText.PointerPressed += OpenAction;

        DetailConnectionBtn.PointerPressed += OpenAction;
        DetailConnectionIcon.PointerPressed += OpenAction;
        DetailConnectionText.PointerPressed += OpenAction;
    }

    private void DeleteAction(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        e.Handled = false;
    }

    private void OpenAction(object? sender, PointerPressedEventArgs e)
    {
        ConnectionViewModel c = (ConnectionViewModel)this.DataContext!;
        ConnectionViewModel.UpdateLastConnection(c.Id);
        e.Handled = false;
    }
}