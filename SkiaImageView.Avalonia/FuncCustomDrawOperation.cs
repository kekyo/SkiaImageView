﻿using Avalonia;
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

    public void Render(ImmediateDrawingContext context)
    {
        var leaseFeature = context.TryGetFeature<ISkiaSharpApiLeaseFeature>();
        if (leaseFeature is null)
        {
            return;
        }
        using var lease = leaseFeature.Lease();
        var canvas = lease?.SkCanvas;
        if (canvas is not null)
        {
            _draw(canvas, SKRect);
        }
    }
}