////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView - Easy way showing SkiaSharp-based image objects onto UI applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;

namespace SkiaImageView.Sample.Models
{
    public sealed class RedditPost
    {
        public readonly string Title;
        public readonly Uri Url;
        public readonly int Score;

        public RedditPost(string title, Uri url, int score)
        {
            this.Title = title;
            this.Url = url;
            this.Score = score;
        }
    }
}
