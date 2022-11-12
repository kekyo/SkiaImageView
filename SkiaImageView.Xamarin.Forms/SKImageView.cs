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
            nameof(Source), null, (d, o, n) => d.OnBitmapChanged());

    public static readonly DependencyProperty ProjectionQualityProperty =
        Interops.Register<object?, SKImageView>(
            nameof(ProjectionQuality),
            ProjectionQuality.Middle,
            (_, _, _) => { });

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
