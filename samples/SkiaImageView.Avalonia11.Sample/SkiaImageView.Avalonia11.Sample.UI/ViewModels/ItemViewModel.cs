////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView - Easy way showing SkiaSharp-based image objects onto UI applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Epoxy;
using SkiaSharp;

namespace SkiaImageView.Sample.ViewModels;

[ViewModel]
public sealed class ItemViewModel
{
    public string? Title { get; set; }

    public SKBitmap? Image { get; set; }

    public int Score { get; set; }
}
