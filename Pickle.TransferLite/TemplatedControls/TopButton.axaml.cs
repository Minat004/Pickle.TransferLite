using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace Pickle.TransferLite.TemplatedControls;

public class TopButton : Button
{
    public static readonly StyledProperty<StreamGeometry> IconProperty =
        AvaloniaProperty.Register<TopButton, StreamGeometry>(nameof(Icon));

    public StreamGeometry Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }
    
    public static readonly StyledProperty<SolidColorBrush> ColorProperty =
        AvaloniaProperty.Register<TopButton, SolidColorBrush>(nameof(Color));

    public SolidColorBrush Color
    {
        get => GetValue(ColorProperty);
        set => SetValue(ColorProperty, value);
    }
}