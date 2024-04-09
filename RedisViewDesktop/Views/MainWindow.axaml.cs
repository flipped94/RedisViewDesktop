using Avalonia.Controls;
using RedisViewDesktop.ViewModels;
using System.Linq;

namespace RedisViewDesktop.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            AddHandler(PointerPressedEvent, Action);
        }
        private void Action(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            Control c = (Control)e.Source!;
            MainWindowViewModel mainWindow = (MainWindowViewModel)this.DataContext!;
            if (c != null && c.Name != null && c.Name.StartsWith("OpenConnection"))
            {
               
                var item = mainWindow.Items.FirstOrDefault(x =>
                {
                    if (x == null) return false;
                    return x.ModelType == typeof(KeysPageViewModel);
                }, null);


                if (item != null)
                {
                    mainWindow.SelectedItem = item;
                }
                else
                {
                    var info = new ItemTemplate(typeof(ServerPageViewModel), "avares://RedisViewDesktop/Assets/more.png");
                    var keys = new ItemTemplate(typeof(KeysPageViewModel), "avares://RedisViewDesktop/Assets/keys.png");
                    mainWindow.Items.Add(info);
                    mainWindow.Items.Add(keys);
                    mainWindow.SelectedItem = keys;
                }
            }
            else if (c != null && c.Name != null && c.Name.StartsWith("DetailConnection"))
            {
                var item = mainWindow.Items.FirstOrDefault(x =>
                {
                    if (x == null) return false;
                    return x.ModelType == typeof(ServerPageViewModel);
                }, null);

                if (item != null)
                {
                    mainWindow.SelectedItem = item;
                }
                else
                {
                    var info = new ItemTemplate(typeof(ServerPageViewModel), "avares://RedisViewDesktop/Assets/more.png");
                    var keys = new ItemTemplate(typeof(KeysPageViewModel), "avares://RedisViewDesktop/Assets/keys.png");
                    mainWindow.Items.Add(info);
                    mainWindow.Items.Add(keys);
                    mainWindow.SelectedItem = info;
                }
            }
            e.Handled = true;
        }
        
    }
}