using System.Diagnostics;
using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;

namespace Pickle.TransferLite.TemplatedControls.Rendering;

public class ArcRender : DrawOperationsBase
{
    public ArcRender(Rect bounds, int stroke, float startAngle, float sweepAngle, Color strokeColor) : base(bounds)
    {
        _stroke = stroke;
        _startAngle = startAngle;
        _sweepAngle = sweepAngle;
        _strokeColor = strokeColor;
    }

    private readonly int _stroke;
    private readonly float _startAngle;
    private readonly float _sweepAngle;
    private readonly Color _strokeColor;

    public override void Render(IDrawingContextImpl drwContext)
    {
        if (drwContext is not ISkiaDrawingContextImpl context) return;
        
        var canvas = context.SkCanvas;

        canvas.Save();

        var info = new SKImageInfo((int)Bounds.Width, (int)Bounds.Height);
        var strokeR = _stroke / 2;
        var rect = new SKRect(strokeR, strokeR, info.Height - strokeR, info.Width - strokeR);

        using (var paint = new SKPaint())
        {
            paint.Shader = SKShader.CreateColor(_strokeColor.ToSKColor());
            paint.Style = SKPaintStyle.Stroke;
            paint.StrokeWidth = _stroke;
            paint.IsAntialias = false;
            paint.Color = _strokeColor.ToSKColor();
            paint.StrokeCap = SKStrokeCap.Round;
            canvas.DrawArc(rect, _startAngle, _sweepAngle, false, paint);
        }

        canvas.Restore();
    }
}