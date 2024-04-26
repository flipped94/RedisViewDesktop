using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.ReactiveUI;
using RedisViewDesktop.ViewModels;

namespace RedisViewDesktop.Views;

public partial class ServerPageView : ReactiveUserControl<ServerPageViewModel>
{
    public ServerPageView()
    {
        InitializeComponent();
    }

    private async void TabStrip_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        TabStrip? tabStrip = sender as TabStrip;
        if (tabStrip is not null)
        {
            TabItem? tabItem = tabStrip.SelectedItem as TabItem;
            string? option = tabItem?.Header as string;           
            if (ViewModel is not null)
            {
                await ViewModel.PrepareData(option);
            }
        }
        e.Handled = true;
    }
}