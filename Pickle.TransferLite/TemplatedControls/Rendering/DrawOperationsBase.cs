using Avalonia;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;

namespace Pickle.TransferLite.TemplatedControls.Rendering;

public class DrawOperationsBase : ICustomDrawOperation
{
    public DrawOperationsBase(Rect bounds)
    {
        Bounds = bounds;
    }
    public void Dispose()
    {
    }

    public bool HitTest(Point p) => true;

    public virtual void Render(IDrawingContextImpl context)
    {
    }

    public Rect Bounds { get; }
    public bool Equals(ICustomDrawOperation? other) => false;
}