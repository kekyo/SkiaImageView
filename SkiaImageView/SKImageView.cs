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
using System.Net.Http;
using System.Threading;

#if WPF
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
#endif

#if XAMARIN_FORMS
using Xamarin.Forms;
using FrameworkElement = Xamarin.Forms.VisualElement;
using DependencyProperty = Xamarin.Forms.BindableProperty;
#endif

namespace SkiaImageView
{
    public enum RenderMode
    {
        Synchronously,
        AsynchronouslyForFetching,
        Asynchronously,
    }

    public sealed class SKImageView : FrameworkElement
    {
        public static readonly DependencyProperty SourceProperty =
            Interops.Register<object?, SKImageView>(
                nameof(Source), null, (d, o, n) => d.OnBitmapChanged());

        public static readonly DependencyProperty StretchProperty =
            Interops.Register<Stretch, SKImageView>(
                nameof(Stretch), Stretch.None, (d, o, n) => d.Invalidate(false));

        public static readonly DependencyProperty StretchDirectionProperty =
            Interops.Register<StretchDirection, SKImageView>(
                nameof(StretchDirection), StretchDirection.UpOnly, (d, o, n) => d.Invalidate(false));

        public static readonly DependencyProperty RenderModeProperty =
            Interops.Register<RenderMode, SKImageView>(
                nameof(RenderMode), RenderMode.AsynchronouslyForFetching, (d, o, n) => d.OnBitmapChanged());

        private static readonly Lazy<HttpClient> httpClient =
            new Lazy<HttpClient>(() => new HttpClient());

        private WriteableBitmap? backingStore;
        private volatile int executionCount;

#if WPF
        private void Invalidate(bool both)
        {
            if (both)
            {
                base.InvalidateMeasure();
            }
            base.InvalidateVisual();
        }
#endif
#if XAMARIN_FORMS
        private void Invalidate(bool both) =>
            base.InvalidateMeasure();
#endif

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

        public RenderMode RenderMode
        {
            get => (RenderMode)this.GetValue(RenderModeProperty);
            set => this.SetValue(RenderModeProperty, value);
        }

        private static WriteableBitmap DrawImage(int width, int height, Action<SKCanvas> action)
        {
            var info = new SKImageInfo(
                width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            var backingStore = new WriteableBitmap(
                info.Width, info.Height, 96.0, 96.0, PixelFormats.Pbgra32, null);

            backingStore.Lock();
            using (var surface = SKSurface.Create(info, backingStore.BackBuffer, backingStore.BackBufferStride))
            {
                action(surface.Canvas);
            }
            backingStore.Unlock();
            backingStore.Freeze();

            return backingStore;
        }

        private async void FetchFromUrl(Uri url, int executionCount)
        {
            if (this.RenderMode != RenderMode.Synchronously)
            {
                // Offloading fetch process.
                using var stream = await httpClient.Value.GetStreamAsync(url).
                    ConfigureAwait(false);

                var bmp = SKBitmap.Decode(stream);
                var backingStore = DrawImage(
                    bmp.Width, bmp.Height,
                    canvas => canvas.DrawBitmap(bmp, default(SKPoint)));

                // Switch to UI thread.
                this.Dispatcher.InvokeAsynchronously(() =>
                {
                    // Avoid race condition.
                    if (executionCount == this.executionCount)
                    {
                        this.backingStore = backingStore;
                        this.Invalidate(true);
                    }
                }, false);
            }
            else
            {
                // DIRTY: For synchronously operation.
                // It's pseudo synchronously operation between beginning and finished.
                // Next of GetResult is onto worker thread.
                using var stream = httpClient.Value.GetStreamAsync(url).
                    ConfigureAwait(false).GetAwaiter().GetResult();

                var bmp = SKBitmap.Decode(stream);
                var backingStore = DrawImage(
                    bmp.Width, bmp.Height,
                    canvas => canvas.DrawBitmap(bmp, default(SKPoint)));

                // Switch to UI thread.
                this.Dispatcher.InvokeAsynchronously(() =>
                {
                    // Avoid race condition.
                    if (executionCount == this.executionCount)
                    {
                        this.backingStore = backingStore;
                        this.Invalidate(true);
                    }
                }, true);   // Higher priority
            }
        }

        private void DrawImageAndSet(
            int width, int height, int executionCount, Action<SKCanvas> action)
        {
            if (this.RenderMode == RenderMode.Asynchronously)
            {
                ThreadPool.QueueUserWorkItem(_ =>
                {
                    var backingStore = DrawImage(width, height, action);

                    this.Dispatcher.InvokeAsynchronously(() =>
                    {
                        // Avoid race condition.
                        if (executionCount == this.executionCount)
                        {
                            this.backingStore = backingStore;
                            this.Invalidate(true);
                        }
                    });
                }, false);
            }
            else
            {
                this.backingStore = DrawImage(width, height, action);
                this.Invalidate(true);
            }
        }

        private void OnBitmapChanged()
        {
            var executionCount = Interlocked.Increment(ref this.executionCount);

            if (this.Source is SKBitmap bitmap)
            {
                this.DrawImageAndSet(
                    bitmap.Width, bitmap.Height, executionCount,
                    canvas => canvas.DrawBitmap(bitmap, default(SKPoint)));
            }
            else if (this.Source is SKImage image)
            {
                this.DrawImageAndSet(
                    image.Width, image.Height, executionCount,
                    canvas => canvas.DrawImage(image, default(SKPoint)));
            }
            else if (this.Source is SKPicture picture)
            {
                this.DrawImageAndSet(
                    (int)base.RenderSize.Width, (int)base.RenderSize.Height, executionCount,
                    canvas => canvas.DrawPicture(picture, default(SKPoint)));
            }
            else if (this.Source is SKDrawable drawable)
            {
                this.DrawImageAndSet(
                    (int)base.RenderSize.Width, (int)base.RenderSize.Height, executionCount,
                    canvas => canvas.DrawDrawable(drawable, default(SKPoint)));
            }
            else if (this.Source is SKSurface surface)
            {
                this.DrawImageAndSet(
                    (int)base.RenderSize.Width, (int)base.RenderSize.Height, executionCount,
                    canvas => canvas.DrawSurface(surface, default(SKPoint)));
            }
            else if (this.Source is string urlString)
            {
                this.backingStore = null;
                this.Invalidate(false);
                this.FetchFromUrl(new Uri(urlString), executionCount);
            }
            else if (this.Source is Uri url)
            {
                this.backingStore = null;
                this.Invalidate(false);
                this.FetchFromUrl(url, executionCount);
            }
            else if (this.Source != null)
            {
                Trace.WriteLine(
                    $"SKImageView: Unknown image type, ignored.: {this.Source.GetType().FullName}");
                this.backingStore = null;
                this.Invalidate(false);
            }
            else
            {
                this.backingStore = null;
                this.Invalidate(false);
            }
        }

        private Size InternalMeasureArrangeOverride(Size targetSize)
        {
            if (this.backingStore is { } backingStore)
            {
                var scaleFactor = Internals.ComputeScaleFactor(
                    targetSize,
                    new Size(backingStore.Width, backingStore.Height),
                    this.Stretch,
                    this.StretchDirection);
                return new Size(
                    backingStore.Width * scaleFactor.Width,
                    backingStore.Height * scaleFactor.Height);
            }
            else
            {
                return default;
            }
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
                drawingContext.DrawImage(backingStore, new Rect(new Point(), base.RenderSize));
            }
        }
    }
}
