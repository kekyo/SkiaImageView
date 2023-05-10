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

    public FuncCustomDrawOperation(SKRect skRect, Action<SKCanvas, SKRect> draw)
    {
        _draw = draw;
        Bounds = new Rect(skRect.Left, skRect.Top, skRect.Width, skRect.Height);
        SKRect = skRect;
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