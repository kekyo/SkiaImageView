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
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using SkiaSharp;

namespace SkiaImageView
{
    internal sealed class BackingStore
    {
        private readonly SKImageInfo imageInfo;
        private readonly WriteableBitmap writableBitmap;
        private ILockedFramebuffer? locker;

        public BackingStore(
            int width, int height, ProjectionQuality projectionQuality)
        {
            this.imageInfo = new(
                width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            this.writableBitmap = new(
                new(imageInfo.Width, imageInfo.Height), new(96.0, 96.0), PixelFormat.Bgra8888, AlphaFormat.Premul);
            this.locker = this.writableBitmap.Lock();
        }

        public Size Size =>
            new(this.imageInfo.Width, this.imageInfo.Height);

        public void Finish()
        {
            this.locker!.Dispose();
            this.locker = null;
        }

        public SKSurface GetSurface() =>
            SKSurface.Create(this.imageInfo, this.locker!.Address, this.locker!.RowBytes);

        public void Draw(DrawingContext drawingContext, Size renderSize) =>
            drawingContext.DrawImage(this.writableBitmap, new(new(), renderSize));
    }
}
