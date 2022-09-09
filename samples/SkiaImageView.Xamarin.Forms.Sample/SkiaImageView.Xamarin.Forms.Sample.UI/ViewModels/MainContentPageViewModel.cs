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
using Epoxy.Synchronized;
using SkiaImageView.Sample.Models;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
// Conflicted between Xamarin.Forms.Command and Epoxy.Command.
using Command = Epoxy.Command;

namespace SkiaImageView.Sample.ViewModels
{
    [ViewModel]
    public sealed class MainContentPageViewModel
    {
        public MainContentPageViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();

            // A handler for page appearing
            this.Ready = Command.Factory.CreateSync(() =>
            {
                this.IsEnabled = true;
            });

            // A handler for fetch button
            this.Fetch = CommandFactory.Create(async () =>
            {
                this.IsEnabled = false;

                try
                {
                    // Uses Reddit API
                    var reddits = await Reddit.FetchNewPostsAsync("r/aww");

                    this.Items.Clear();

                    static async ValueTask<SKBitmap?> FetchImageAsync(Uri url) =>
                        SKBitmap.Decode(await Reddit.FetchImageAsync(url));

                    foreach (var reddit in reddits.Take(3))
                    {
                        this.Items.Add(new ItemViewModel
                        {
                            Title = reddit.Title,
                            Score = reddit.Score,
                            Image = await FetchImageAsync(reddit.Url),
                            //Image = reddit.Url,
                        });
                    }
                }
                finally
                {
                    IsEnabled = true;
                }
            });
        }

        public Command? Ready { get; private set; }

        public bool IsEnabled { get; private set; }

        public ObservableCollection<ItemViewModel>? Items { get; private set; }

        public Epoxy.Command? Fetch { get; private set; }
    }
}
