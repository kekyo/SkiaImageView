////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView - Easy way showing SkiaSharp-based image objects onto UI applications.
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

    public sealed class SKImageView :
#if WPF
        FrameworkElement
#elif XAMARIN_FORMS
        Image
#endif
    {
#if WPF
        public static readonly DependencyProperty SourceProperty =
            Interops.Register<object?, SKImageView>(
                nameof(Source), null, (d, o, n) => d.OnBitmapChanged());
#elif XAMARIN_FORMS
        public static new readonly DependencyProperty SourceProperty =
            Interops.Register<object?, SKImageView>(
                nameof(Source), null, (d, o, n) => d.OnBitmapChanged());
#endif
        public static readonly DependencyProperty StretchProperty =
            Interops.Register<Stretch, SKImageView>(
                nameof(Stretch), Stretch.None, (d, o, n) => d.Invalidate(false));

        public static readonly DependencyProperty StretchDirectionProperty =
            Interops.Register<StretchDirection, SKImageView>(
                nameof(StretchDirection), StretchDirection.Both, (d, o, n) => d.Invalidate(false));

        public static readonly DependencyProperty RenderModeProperty =
            Interops.Register<RenderMode, SKImageView>(
                nameof(RenderMode), RenderMode.AsynchronouslyForFetching, (d, o, n) => d.OnBitmapChanged());

        private static readonly Lazy<HttpClient> httpClient =
            new Lazy<HttpClient>(() => new HttpClient());

        private BackingStore? backingStore;
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

        public object Source
        {
            get => this.GetValue(SourceProperty);
            set => this.SetValue(SourceProperty, value);
        }
#elif XAMARIN_FORMS
        public SKImageView()
        {
            base.Aspect = Aspect.AspectFill;
        }

        private void Invalidate(bool both) =>
            base.InvalidateMeasure();

        public new object Source
        {
            get => this.GetValue(SourceProperty);
            set => this.SetValue(SourceProperty, value);
        }
#endif

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

#if WPF
        private void UpdateWith(BackingStore? backingStore)
        {
            this.backingStore = backingStore;
            this.Invalidate(true);
        }
#elif XAMARIN_FORMS
        private Size RenderSize =>
            new Size(base.Width, base.Height);

        private void UpdateWith(BackingStore? backingStore)
        {
            this.backingStore = backingStore;
            base.Source = this.backingStore?.GetImageSource();
        }
#endif

        private static BackingStore DrawImage(int width, int height, Action<SKCanvas> action)
        {
            var info = new SKImageInfo(
                width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            var backingStore = new BackingStore(info.Width, info.Height);

            using (var surface = backingStore.GetSurface())
            {
                action(surface.Canvas);
            }

            backingStore.Finish();

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
                        this.UpdateWith(backingStore);
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
                        this.UpdateWith(backingStore);
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
                            this.UpdateWith(backingStore);
                        }
                    }, false);
                }, false);
            }
            else
            {
                var backingStore = DrawImage(width, height, action);
                this.UpdateWith(backingStore);
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
                var renderSize = this.RenderSize;
                this.DrawImageAndSet(
                    (int)renderSize.Width, (int)renderSize.Height, executionCount,
                    canvas => canvas.DrawPicture(picture, default(SKPoint)));
            }
            else if (this.Source is SKDrawable drawable)
            {
                var renderSize = this.RenderSize;
                this.DrawImageAndSet(
                    (int)renderSize.Width, (int)renderSize.Height, executionCount,
                    canvas => canvas.DrawDrawable(drawable, default(SKPoint)));
            }
            else if (this.Source is SKSurface surface)
            {
                var renderSize = this.RenderSize;
                this.DrawImageAndSet(
                    (int)renderSize.Width, (int)renderSize.Height, executionCount,
                    canvas => canvas.DrawSurface(surface, default(SKPoint)));
            }
            else if (this.Source is string urlString)
            {
                this.UpdateWith(null);
                this.FetchFromUrl(new Uri(urlString), executionCount);
            }
            else if (this.Source is Uri url)
            {
                this.UpdateWith(null);
                this.FetchFromUrl(url, executionCount);
            }
            else if (this.Source != null)
            {
                Trace.WriteLine(
                    $"SKImageView: Unknown image type, ignored: {this.Source.GetType().FullName}");
                this.UpdateWith(null);
            }
            else
            {
                this.UpdateWith(null);
            }
        }

        private Size InternalMeasureArrangeOverride(Size targetSize)
        {
            if (this.backingStore is { } backingStore)
            {
                var scaleFactor = Internals.ComputeScaleFactor(
                    targetSize,
                    backingStore.Size,
                    this.Stretch,
                    this.StretchDirection);
                return new Size(
                    backingStore.Size.Width * scaleFactor.Width,
                    backingStore.Size.Height * scaleFactor.Height);
            }
            else
            {
                return default;
            }
        }

#if WPF
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
#elif XAMARIN_FORMS
        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint) =>
            new SizeRequest(InternalMeasureArrangeOverride(new Size(widthConstraint, heightConstraint)));
#endif
    }
}
