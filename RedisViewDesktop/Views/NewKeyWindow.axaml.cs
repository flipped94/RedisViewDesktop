using Avalonia.ReactiveUI;
using AvaloniaEdit.Utils;
using ReactiveUI;
using RedisViewDesktop.Models;
using RedisViewDesktop.ViewModels;
using System.Collections.ObjectModel;

namespace RedisViewDesktop.Views;

public partial class NewKeyWindow : ReactiveWindow<NewKeyViewModel>
{
    public NewKeyWindow()
    {
        InitializeComponent();

        this.WhenActivated(action =>
        {
            var newKeys = new ObservableCollection<NewKey>();
            newKeys.AddRange(NewKey.GetNewKeys());
            KeyComBox.ItemsSource = newKeys;
            KeyComBox.SelectedIndex = 0;
            action(ViewModel!.AddComand.Subscribe(Close));
            action(ViewModel!.CancelCommand.Subscribe(Close));
        });
    }

}