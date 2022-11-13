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
using Xamarin.Forms;

namespace SkiaImageView
{
    internal sealed class BackingStore
    {
        // HACK: XF requires custom renderrer for drawing independent any bitmap.
        //   It is force applicable limitation for packaging strategy, be going to separates between platforms.
        //   So, avoid it by will re-encode bitmap to image stream with PNG format in ImageSource object...
        private readonly SKImageInfo imageInfo;
        private readonly ProjectionQuality projectionQuality;
        private SKBitmap? bitmap;
        private ImageSource? imageSource;

        public BackingStore(
            int width, int height, ProjectionQuality projectionQuality)
        {
            this.projectionQuality = projectionQuality;
            this.imageInfo = new(
                width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            this.bitmap = new(
                imageInfo.Width, imageInfo.Height, SKColorType.Rgba8888, SKAlphaType.Premul);
        }

        public BackingStore(Uri url) =>
            this.imageSource = ImageSource.FromUri(url);

        public Size Size =>
            new(this.imageInfo.Width, this.imageInfo.Height);

        public void Finish()
        {
            // Xamarin Forms is in essence, supposed to be used on smartphones and tablets,
            // so a Middle quality of 90 does not seem too out of place.
            // The amount of data can be significantly reduced.
            var (format, quality) = this.projectionQuality switch
            {
                ProjectionQuality.Perfect => (SKEncodedImageFormat.Png, 100),
                ProjectionQuality.Low => (SKEncodedImageFormat.Jpeg, 80),
                ProjectionQuality.High => (SKEncodedImageFormat.Jpeg, 95),
                _ => (SKEncodedImageFormat.Jpeg, 90),
            };

            var bitmapData = this.bitmap!.Encode(format, quality);
            this.bitmap.Dispose();
            this.bitmap = null;
            this.imageSource = ImageSource.FromStream(() => bitmapData.AsStream());
        }

        public SKSurface GetSurface() =>
            SKSurface.Create(this.imageInfo, this.bitmap!.GetAddress(0, 0), this.bitmap.RowBytes);

        public ImageSource GetImageSource() =>
            this.imageSource!;
    }
}
