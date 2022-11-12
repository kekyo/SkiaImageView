////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView - Easy way showing SkiaSharp-based image objects onto UI applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Xamarin.Forms;
using DependencyProperty = Xamarin.Forms.BindableProperty;

namespace SkiaImageView;

partial class SKImageView : Image
{
    public static new readonly DependencyProperty SourceProperty =
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
            ProjectionQuality.Middle,   // Limited on XF
            _ => { });

    public SKImageView() =>
        base.Aspect = Aspect.AspectFill;

    private void Invalidate(bool both) =>
        base.InvalidateMeasure();

    public new object Source
    {
        get => this.GetValue(SourceProperty);
        set => this.SetValue(SourceProperty, value);
    }

    private Size RenderSize =>
        new Size(base.Width, base.Height);

    private void UpdateWith(BackingStore? backingStore)
    {
        this.backingStore = backingStore;
        base.Source = this.backingStore?.GetImageSource();
    }

    protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint) =>
        new SizeRequest(this.InternalMeasureArrangeOverride(new Size(widthConstraint, heightConstraint)));
}
