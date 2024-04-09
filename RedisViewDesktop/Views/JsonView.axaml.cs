using Avalonia.ReactiveUI;
using AvaloniaEdit.TextMate;
using ReactiveUI;
using RedisViewDesktop.ViewModels;
using TextMateSharp.Grammars;

namespace RedisViewDesktop.Views;

public partial class JsonView : ReactiveUserControl<JsonViewModel>
{
    public JsonView()
    {
        InitializeComponent();
        var registryOptions = new RegistryOptions(ThemeName.Light);
        var textMateInstallation = Editor.InstallTextMate(registryOptions);
        textMateInstallation.SetGrammar(registryOptions.GetScopeByLanguageId(registryOptions.GetLanguageByExtension(".json").Id));

        this.WhenActivated(action =>
        {
            ViewModel!.Ready(Editor);
        });
    }

}
