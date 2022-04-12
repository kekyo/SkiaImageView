////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView.Wpf - Easy way showing SkiaSharp-based image objects onto WPF applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using SkiaSharp;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SkiaImageView
{
    public sealed class SKImageView : FrameworkElement
    {
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                nameof(Source), typeof(object), typeof(SKImageView),
                new PropertyMetadata(
                    null, (s, e) => ((SKImageView)s).OnBitmapChanged(e)));

        public static readonly DependencyProperty StretchProperty =
            DependencyProperty.Register(
                nameof(Stretch), typeof(Stretch), typeof(SKImageView),
                new PropertyMetadata(
                    Stretch.Uniform, (s, e) => ((SKImageView)s).InvalidateVisual()));

        public static readonly DependencyProperty HorizontalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(HorizontalContentAlignment), typeof(HorizontalAlignment), typeof(SKImageView),
                new PropertyMetadata(
                    HorizontalAlignment.Center, (s, e) => ((SKImageView)s).InvalidateVisual()));

        public static readonly DependencyProperty VerticalContentAlignmentProperty =
            DependencyProperty.Register(
                nameof(VerticalContentAlignment), typeof(VerticalAlignment), typeof(SKImageView),
                new PropertyMetadata(
                    VerticalAlignment.Center, (s, e) => ((SKImageView)s).InvalidateVisual()));

        private WriteableBitmap? backingStore;

        public SKImageView() =>
            this.Stretch = Stretch.Uniform;

        public object Source
        {
            get => this.GetValue(SourceProperty);
            set => this.SetValue(SourceProperty, value);
        }

        public Stretch Stretch
        {
            get => (Stretch)this.GetValue(StretchProperty);
            set => this.SetValue(StretchProperty, value);
        }

        public HorizontalAlignment HorizontalContentAlignment
        {
            get => (HorizontalAlignment)this.GetValue(HorizontalContentAlignmentProperty);
            set => this.SetValue(HorizontalContentAlignmentProperty, value);
        }

        public VerticalAlignment VerticalContentAlignment
        {
            get => (VerticalAlignment)this.GetValue(VerticalContentAlignmentProperty);
            set => this.SetValue(VerticalContentAlignmentProperty, value);
        }

        private void OnBitmapChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Source is SKBitmap bitmap)
            {
                var info = new SKImageInfo(
                    bitmap.Width, bitmap.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                var backingStore = new WriteableBitmap(
                    info.Width, info.Height, 96.0 * 1, 96.0 * 1, PixelFormats.Pbgra32, null);

                using (var surface = SKSurface.Create(info, backingStore.BackBuffer, backingStore.BackBufferStride))
                {
                    surface.Canvas.DrawBitmap(bitmap, default(SKPoint));
                }
                backingStore.Freeze();

                this.backingStore = backingStore;
            }
            else if (this.Source is SKImage image)
            {
                var info = new SKImageInfo(
                    image.Width, image.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                var backingStore = new WriteableBitmap(
                    info.Width, info.Height, 96.0 * 1, 96.0 * 1, PixelFormats.Pbgra32, null);

                using (var surface = SKSurface.Create(info, backingStore.BackBuffer, backingStore.BackBufferStride))
                {
                    surface.Canvas.DrawImage(image, default(SKPoint));
                }
                backingStore.Freeze();

                this.backingStore = backingStore;
            }
            else if (this.Source is SKDrawable drawable)
            {
                var info = new SKImageInfo(
                    (int)base.RenderSize.Width, (int)base.RenderSize.Height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
                var backingStore = new WriteableBitmap(
                    info.Width, info.Height, 96.0 * 1, 96.0 * 1, PixelFormats.Pbgra32, null);

                using (var surface = SKSurface.Create(info, backingStore.BackBuffer, backingStore.BackBufferStride))
                {
                    surface.Canvas.DrawDrawable(drawable, default(SKPoint));
                }
                backingStore.Freeze();

                this.backingStore = backingStore;
            }
            else
            {
                this.backingStore = null;
            }

            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (this.backingStore is { } backingStore)
            {
                if (backingStore.Width > 0 && backingStore.Height > 0 &&
                    base.RenderSize.Width > 0 && base.RenderSize.Height > 0)
                {
                    switch (this.Stretch)
                    {
                        case Stretch.Uniform:
                            Rect rect;
                            if (((double)base.RenderSize.Width / base.RenderSize.Height) >
                                ((double)backingStore.Width / backingStore.Height))
                            {
                                var width = (double)base.RenderSize.Height / backingStore.Height *
                                    backingStore.Width;
                                var left = (base.RenderSize.Width - width) / 2;

                                rect = new Rect(left, 0, width, base.RenderSize.Height);
                            }
                            else
                            {
                                var height = (double)base.RenderSize.Width / backingStore.Width *
                                    backingStore.Height;
                                var top = (base.RenderSize.Height - height) / 2;

                                rect = new Rect(0, top, base.RenderSize.Width, height);
                            }
                            drawingContext.DrawImage(backingStore, rect);
                            break;

                        default:
                            drawingContext.DrawImage(
                                backingStore,
                                new Rect(
                                    (base.RenderSize.Width - backingStore.Width) / 2,
                                    (base.RenderSize.Height - backingStore.Height) / 2,
                                    backingStore.Width,
                                    backingStore.Height));
                            break;
                    }
                }
            }
        }
    }
}
