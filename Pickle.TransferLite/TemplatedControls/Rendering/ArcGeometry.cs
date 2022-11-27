using Avalonia;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Platform;

namespace Pickle.TransferLite.TemplatedControls.Rendering;

public class ArcGeometry : DrawOperationsBase
{
    public ArcGeometry(Rect bounds, IBrush brush, int strokeThickness, float startAngle, float sweepAngle) : base(bounds)
    {
        _brush = brush;
        _strokeThickness = strokeThickness;
    }

    private readonly IBrush _brush;
    private readonly int _strokeThickness;

    public override void Render(IDrawingContextImpl drwContext)
    {
        if (drwContext is not StreamGeometry context) return;

        var strokeR = _strokeThickness / 2;
        var xStart = Bounds.Width / 2;
        var yStart = strokeR;
        var xEnd = Bounds.Width / strokeR;
        var yEnd = Bounds.Height - strokeR;
        var streamGeometry = new StreamGeometry();
        
        var path = new Path
        {
            Stroke = _brush,
            StrokeThickness = _strokeThickness
        };

        using (var contextImpl = context.Open())
        {
            contextImpl.BeginFigure(new Point(xStart, yStart), false);
            contextImpl.ArcTo(new Point(xEnd, yEnd), new Size(Bounds.Width / 2, Bounds.Height), 20, true, SweepDirection.Clockwise);
        }
        

        path.Data = context;
    }
}