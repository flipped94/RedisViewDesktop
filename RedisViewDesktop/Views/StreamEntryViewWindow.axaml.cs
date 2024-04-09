using Avalonia.ReactiveUI;
using AvaloniaEdit.TextMate;
using ReactiveUI;
using RedisViewDesktop.ViewModels;
using TextMateSharp.Grammars;

namespace RedisViewDesktop.Views;

public partial class StreamEntryViewWindow : ReactiveWindow<StreamEntryViewViewModel>
{
    public StreamEntryViewWindow()
    {
        InitializeComponent();
        
        this.WhenActivated(action =>
        {
            var registryOptions = new RegistryOptions(ThemeName.Light);
            var textMateInstallation = Editor.InstallTextMate(registryOptions);
            textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".json").Id));
            Editor.Text = ViewModel!.Value;
        });
    }
}
