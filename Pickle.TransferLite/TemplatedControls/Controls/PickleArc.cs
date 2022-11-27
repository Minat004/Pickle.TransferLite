using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using Avalonia.Threading;
using Pickle.TransferLite.TemplatedControls.Rendering;

namespace Pickle.TransferLite.TemplatedControls.Controls;

public class PickleArc : Control
{
    private Brush _brush = new SolidColorBrush();
    static PickleArc()
    {
        AffectsRender<PickleArc>(StartAngleProperty,
            SweepAngleProperty,
            StrokeProperty,
            PickleColorProperty);    
    }
    
    public static readonly StyledProperty<double> StartAngleProperty =
        AvaloniaProperty.Register<PickleArc, double>(nameof(StartAngle), -90D, true, BindingMode.TwoWay);
    public double StartAngle
    {
        get => GetValue(StartAngleProperty);
        set => SetValue(StartAngleProperty, value);
    }

    public static readonly StyledProperty<double> SweepAngleProperty =
        AvaloniaProperty.Register<PickleArc, double>(nameof(SweepAngle), 180D, true, BindingMode.TwoWay);
    public double SweepAngle
    {
        get => GetValue(SweepAngleProperty);
        set => SetValue(SweepAngleProperty, value);
    }

    public static readonly StyledProperty<Color> PickleColorProperty =
        AvaloniaProperty.Register<PickleArc, Color>(nameof(PickleColor), Colors.Azure, true, BindingMode.TwoWay);
    public Color PickleColor
    {
        get => GetValue(PickleColorProperty);
        set => SetValue(PickleColorProperty, value);
    }

    public static readonly StyledProperty<int> StrokeProperty =
        AvaloniaProperty.Register<PickleArc, int>(nameof(Stroke), 10, true, BindingMode.TwoWay);
    public int Stroke
    {
        get => GetValue(StrokeProperty);
        set => SetValue(StrokeProperty, value);
    }

    public override void Render(DrawingContext context)
    {
        var diameter = Bounds.Width <= Bounds.Height ? Bounds.Width : Bounds.Height;
        context.Custom(new ArcRender(new Rect(0, 0, diameter, diameter), Stroke, (float)StartAngle, (float)SweepAngle, PickleColor));
        // context.Custom(new ArcGeometry(new Rect(0,0,Bounds.Width, Bounds.Height), _brush, Stroke, 0, 180));
        Dispatcher.UIThread.InvokeAsync(InvalidateVisual, DispatcherPriority.Background);
    }
}