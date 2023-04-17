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
using SkiaImageView.Sample.Models;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace SkiaImageView.Sample.ViewModels;

public sealed class MainWindowViewModel : ReactiveObject
{
    public MainWindowViewModel()
    {
        this.Items = new ObservableCollection<ItemViewModel>();
    }

    public void Ready()
    {
        this.IsEnabled = true;
    }

    [Reactive]
    public bool IsEnabled { get; private set; }

    [Reactive]
    public ObservableCollection<ItemViewModel>? Items { get; private set; }

    public async Task Fetch()
    {
        this.IsEnabled = false;

        try
        {
            // Uses Reddit API
            var reddits = await Reddit.FetchNewPostsAsync("r/aww");

            this.Items?.Clear();

            static async ValueTask<SKBitmap?> FetchImageAsync(Uri url) =>
                SKBitmap.Decode(await Reddit.FetchImageAsync(url));

            foreach (var reddit in reddits)
            {
                this.Items?.Add(new ItemViewModel
                {
                    Title = reddit.Title,
                    Score = reddit.Score,
                    Image = await FetchImageAsync(reddit.Url)
                });
            }
        }
        finally
        {
            IsEnabled = true;
        }
    }
}
