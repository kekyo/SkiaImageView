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

namespace SkiaImageView;

partial class SKImageView : Image
{
    public static new readonly BindableProperty SourceProperty =
        Interops.Register<object?, SKImageView>(
            nameof(Source), null, d => d.OnBitmapChanged());

    public static readonly BindableProperty StretchProperty =
        Interops.Register<Stretch, SKImageView>(
            nameof(Stretch), Stretch.None, d => d.Invalidate(false));

    public static readonly BindableProperty StretchDirectionProperty =
        Interops.Register<StretchDirection, SKImageView>(
            nameof(StretchDirection), StretchDirection.Both, d => d.Invalidate(false));

    public static readonly BindableProperty RenderModeProperty =
        Interops.Register<RenderMode, SKImageView>(
            nameof(RenderMode), RenderMode.AsynchronouslyForFetching, d => d.OnBitmapChanged());

    public static readonly BindableProperty ProjectionQualityProperty =
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

    public Size RenderSize =>
        new(base.Width, base.Height);

    private void UpdateWith(BackingStore? backingStore)
    {
        this.backingStore = backingStore;
        base.Source = this.backingStore?.GetImageSource();
    }

    protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint) =>
        new(this.InternalMeasureArrangeOverride(new(widthConstraint, heightConstraint)));
}
