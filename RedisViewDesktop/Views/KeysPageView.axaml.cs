using Avalonia.ReactiveUI;
using RedisViewDesktop.ViewModels;

namespace RedisViewDesktop.Views;

public partial class KeysPageView : ReactiveUserControl<KeysPageViewModel>
{
    public KeysPageView()
    {
        InitializeComponent();
    }

    private void ScannedKeys_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        KeysScrollViewer.ScrollToEnd();
    }    
}