using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Pickle.TransferLite.Views;

public partial class DirectoryPanelView : UserControl
{
    public DirectoryPanelView()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}