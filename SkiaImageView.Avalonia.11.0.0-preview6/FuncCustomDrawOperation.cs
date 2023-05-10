using Avalonia;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using SkiaSharp;
using System;

namespace SkiaImageView;

internal class FuncCustomDrawOperation : ICustomDrawOperation
{
    private readonly Action<SKCanvas, SKRect> _draw;

    public FuncCustomDrawOperation(Rect bounds, Action<SKCanvas, SKRect> draw)
    {
        _draw = draw;
        Bounds = bounds;
        SKRect = new SKRect { Left = (float)Bounds.Left, Right = (float)Bounds.Right, Top = (float)Bounds.Top, Bottom = (float)Bounds.Bottom };
    }

    public void Dispose()
    {
    }

    public Rect Bounds { get; }

    public SKRect SKRect { get; }

    public bool HitTest(Point p) => Bounds.Contains(p);

    public bool Equals(ICustomDrawOperation? other)
    {
        return object.ReferenceEquals(this, other);
    }

#if Preview6
    public void Render(IDrawingContextImpl context)
    {
        var leaseFeature = context.GetFeature<ISkiaSharpApiLeaseFeature>();
        RenderLeaseFeature(leaseFeature);
    }
#endif
#if Preview8
    public void Render(ImmediateDrawingContext context)
    {
        var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
        RenderLeaseFeature(leaseFeature);
    }
#endif

    private void RenderLeaseFeature(ISkiaSharpApiLeaseFeature? leaseFeature)
    {
        if (leaseFeature is { })
        {
            using var lease = leaseFeature.Lease();
            var canvas = lease?.SkCanvas;
            if (canvas is not null)
            {
                _draw(canvas, SKRect);
            }
        }
    }
}