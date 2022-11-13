////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView - Easy way showing SkiaSharp-based image objects onto UI applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SkiaImageView;

partial class SKImageView : FrameworkElement
{
    public static readonly DependencyProperty SourceProperty =
        Interops.Register<object?, SKImageView>(
            nameof(Source), null, d => d.OnBitmapChanged());

    public static readonly DependencyProperty StretchProperty =
        Interops.Register<Stretch, SKImageView>(
            nameof(Stretch), Stretch.None, d => d.Invalidate(false));

    public static readonly DependencyProperty StretchDirectionProperty =
        Interops.Register<StretchDirection, SKImageView>(
            nameof(StretchDirection), StretchDirection.Both, d => d.Invalidate(false));

    public static readonly DependencyProperty RenderModeProperty =
        Interops.Register<RenderMode, SKImageView>(
            nameof(RenderMode), RenderMode.AsynchronouslyForFetching, d => d.OnBitmapChanged());

    public static readonly DependencyProperty ProjectionQualityProperty =
        Interops.Register<ProjectionQuality, SKImageView>(
            nameof(ProjectionQuality),
            ProjectionQuality.Perfect,
            _ => { });

    private void Invalidate(bool both)
    {
        if (both)
        {
            base.InvalidateMeasure();
        }
        base.InvalidateVisual();
    }

    public object Source
    {
        get => this.GetValue(SourceProperty);
        set => this.SetValue(SourceProperty, value);
    }

    private void UpdateWith(BackingStore? backingStore)
    {
        this.backingStore = backingStore;
        this.Invalidate(true);
    }

    protected override Size MeasureOverride(Size constraint) =>
        this.InternalMeasureArrangeOverride(constraint);

    protected override Size ArrangeOverride(Size arrangeSize) =>
        this.InternalMeasureArrangeOverride(arrangeSize);

    protected override void OnRender(DrawingContext drawingContext)
    {
        base.OnRender(drawingContext);
    
        if (this.backingStore is { } backingStore)
        {
            backingStore.Draw(drawingContext, this.RenderSize);
        }
    }
}
