////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView - Easy way showing SkiaSharp-based image objects onto UI applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SkiaSharp;

namespace SkiaImageView.Sample.ViewModels;

public sealed class ItemViewModel : ReactiveObject
{
    [Reactive]
    public string? Title { get; set; }

    [Reactive]
    public SKBitmap? Image { get; set; }

    [Reactive]
    public int Score { get; set; }
}
