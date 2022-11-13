////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView - Easy way showing SkiaSharp-based image objects onto UI applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;

namespace SkiaImageView;

partial class SKImageView : Control
{
    public static readonly AvaloniaProperty<object?> SourceProperty =
        Interops.Register<object?, SKImageView>(
            nameof(Source), null, d => d.OnBitmapChanged());

    public static readonly AvaloniaProperty<Stretch> StretchProperty =
        Interops.Register<Stretch, SKImageView>(
            nameof(Stretch), Stretch.None, d => d.Invalidate(false));

    public static readonly AvaloniaProperty<StretchDirection> StretchDirectionProperty =
        Interops.Register<StretchDirection, SKImageView>(
            nameof(StretchDirection), StretchDirection.Both, d => d.Invalidate(false));

    public static readonly AvaloniaProperty<RenderMode> RenderModeProperty =
        Interops.Register<RenderMode, SKImageView>(
            nameof(RenderMode), RenderMode.AsynchronouslyForFetching, d => d.OnBitmapChanged());

    public static readonly AvaloniaProperty<ProjectionQuality> ProjectionQualityProperty =
        Interops.Register<ProjectionQuality, SKImageView>(
            nameof(ProjectionQuality),
            ProjectionQuality.Perfect,
            _ => { });

    static SKImageView()
    {
        AffectsRender<SKImageView>(SourceProperty, StretchProperty, StretchDirectionProperty);
        AffectsMeasure<SKImageView>(SourceProperty, StretchProperty, StretchDirectionProperty);
    }

    private void Invalidate(bool both)
    {
        if (both)
        {
            base.InvalidateMeasure();
        }
        base.InvalidateVisual();
    }

    [Content]
    public object? Source
    {
        get => this.GetValue(SourceProperty);
        set => this.SetValue(SourceProperty, value);
    }

    private Size RenderSize =>
        this.Bounds.Size;

    private void UpdateWith(BackingStore? backingStore)
    {
        this.backingStore = backingStore;
        this.Invalidate(true);
    }

    protected override Size MeasureOverride(Size constraint) =>
        this.InternalMeasureArrangeOverride(constraint);

    protected override Size ArrangeOverride(Size arrangeSize) =>
        this.InternalMeasureArrangeOverride(arrangeSize);

    public override void Render(DrawingContext drawingContext)
    {
        base.Render(drawingContext);

        if (this.backingStore is { } backingStore)
        {
            backingStore.Draw(drawingContext, this.RenderSize);
        }
    }
}
