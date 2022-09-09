////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView.Wpf - Easy way showing SkiaSharp-based image objects onto WPF applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

namespace SkiaImageView.Sample
{
    public partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();

            LoadApplication(new App());
        }
    }
}
