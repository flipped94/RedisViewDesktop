using Avalonia.ReactiveUI;
using ReactiveUI;
using RedisViewDesktop.ViewModels;

namespace RedisViewDesktop.Views;

public partial class StringView : ReactiveUserControl<StringViewModel>
{
    public StringView()
    {
        InitializeComponent();
        this.WhenActivated((x) =>
        {
            StringViewModel stringViewModel = (StringViewModel)this.DataContext;
            stringViewModel.Ready();
        });
    }
}