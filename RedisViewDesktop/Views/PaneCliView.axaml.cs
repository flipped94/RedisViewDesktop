using Avalonia.Controls;

namespace RedisViewDesktop.Views;

public partial class PaneCliView : UserControl
{
    public PaneCliView()
    {
        InitializeComponent();
    }

    private void TextBox_TextChanged(object? sender, Avalonia.Controls.TextChangedEventArgs e)
    {
        var textBox = (TextBox)sender!;
        if (textBox != null)
        {
            textBox.CaretIndex = null == textBox.Text ? 0 : textBox.Text.Length;
        }
    }
}