using Avalonia.Controls;
using AvaloniaEdit.TextMate;
using RedisViewDesktop.ViewModels;
using TextMateSharp.Grammars;

namespace RedisViewDesktop.Views;

public partial class PaneLuaView : UserControl
{
    public PaneLuaView()
    {
        InitializeComponent();
        var registryOptions = new RegistryOptions(ThemeName.Light);
        var textMateInstallation = Editor.InstallTextMate(registryOptions);
        textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".lua").Id));
    }

    private async void RunScript(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        PaneLuaViewModel paneLuaViewModel = (PaneLuaViewModel)DataContext;
        if (null != paneLuaViewModel)
        {
            await paneLuaViewModel.ExecuteScript(Editor.Text);
        }
    }
}