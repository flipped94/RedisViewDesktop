using Avalonia.Controls;
using RedisViewDesktop.Helpers;

namespace RedisViewDesktop.Views;

public partial class ConnectionStoreView : UserControl
{

    public ConnectionStoreView()
    {
        InitializeComponent();               
    }   

    private void Close(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Control c = (Control)sender!;
        Window window = WindowHelper.FindParentWindow(c)!;
        window!.Close();
    }
}
