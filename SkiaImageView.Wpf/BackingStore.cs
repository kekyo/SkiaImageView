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
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SkiaImageView
{
    internal sealed class BackingStore
    {
        private readonly SKImageInfo imageInfo;
        private readonly WriteableBitmap writableBitmap;

        public BackingStore(
            int width, int height, ProjectionQuality projectionQuality)
        {
            this.imageInfo = new SKImageInfo(
                width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            this.writableBitmap = new WriteableBitmap(
                imageInfo.Width, imageInfo.Height, 96.0, 96.0, PixelFormats.Pbgra32, null);
            this.writableBitmap.Lock();
        }

        public Size Size =>
            new Size(this.imageInfo.Width, this.imageInfo.Height);

        public void Finish()
        {
            this.writableBitmap.Unlock();
            this.writableBitmap.Freeze();
        }

        public SKSurface GetSurface() =>
            SKSurface.Create(this.imageInfo, this.writableBitmap.BackBuffer, this.writableBitmap.BackBufferStride);

        public void Draw(DrawingContext drawingContext, Size renderSize) =>
            drawingContext.DrawImage(this.writableBitmap, new Rect(new Point(), renderSize));
    }
}
