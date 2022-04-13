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
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
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

        public static readonly DependencyProperty StretchDirectionProperty =
            DependencyProperty.Register(
                nameof(StretchDirection), typeof(StretchDirection), typeof(SKImageView),
                new PropertyMetadata(
                    StretchDirection.Both, (s, e) => ((SKImageView)s).InvalidateVisual()));

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

        public StretchDirection StretchDirection
        {
            get => (StretchDirection)this.GetValue(StretchDirectionProperty);
            set => this.SetValue(StretchDirectionProperty, value);
        }

        private void DrawImage(int width, int height, Action<SKCanvas> action)
        {
            var info = new SKImageInfo(
                width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            var backingStore = new WriteableBitmap(
                info.Width, info.Height, 96.0 * 1, 96.0 * 1, PixelFormats.Pbgra32, null);

            using (var surface = SKSurface.Create(info, backingStore.BackBuffer, backingStore.BackBufferStride))
            {
                action(surface.Canvas);
            }

            backingStore.Freeze();
            this.backingStore = backingStore;
        }

        private void OnBitmapChanged(DependencyPropertyChangedEventArgs e)
        {
            if (this.Source is SKBitmap bitmap)
            {
                this.DrawImage(
                    bitmap.Width, bitmap.Height,
                    canvas => canvas.DrawBitmap(bitmap, default(SKPoint)));
            }
            else if (this.Source is SKImage image)
            {
                this.DrawImage(
                    image.Width, image.Height,
                    canvas => canvas.DrawImage(image, default(SKPoint)));
            }
            else if (this.Source is SKPicture picture)
            {
                this.DrawImage(
                    (int)base.RenderSize.Width, (int)base.RenderSize.Height,
                    canvas => canvas.DrawPicture(picture, default(SKPoint)));
            }
            else if (this.Source is SKDrawable drawable)
            {
                this.DrawImage(
                    (int)base.RenderSize.Width, (int)base.RenderSize.Height,
                    canvas => canvas.DrawDrawable(drawable, default(SKPoint)));
            }
            else if (this.Source is SKSurface surface)
            {
                this.DrawImage(
                    (int)base.RenderSize.Width, (int)base.RenderSize.Height,
                    canvas => canvas.DrawSurface(surface, default(SKPoint)));
            }
            else if (this.Source != null)
            {
                Trace.WriteLine(
                    $"SKImageView: Unknown image type, ignored.: {this.Source.GetType().FullName}");
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
