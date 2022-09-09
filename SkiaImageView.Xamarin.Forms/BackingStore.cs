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
        private SKBitmap? bitmap;
        private ImageSource? imageSource;

        public BackingStore(int width, int height)
        {
            this.imageInfo = new SKImageInfo(
                width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
            this.bitmap = new SKBitmap(
                imageInfo.Width, imageInfo.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
        }

        public BackingStore(Uri url) =>
            this.imageSource = ImageSource.FromUri(url);

        public Size Size =>
            new Size(this.imageInfo.Width, this.imageInfo.Height);

        public void Finish()
        {
            var bitmapData = this.bitmap!.Encode(SKEncodedImageFormat.Png, 100);
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
