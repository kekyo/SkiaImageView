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
using Xamarin.Forms;

namespace SkiaImageView.Sample.Views.Converters;

public sealed class ScoreToBrushConverter : ValueConverter<int, Brush>
{
    private static readonly Brush yellow = new SolidColorBrush(Color.FromRgba(96, 96, 0, 255));
    private static readonly Brush gray = new SolidColorBrush(Color.FromRgba(96, 96, 96, 255));

    public override bool TryConvert(int from, out Brush result)
    {
        result = from >= 5 ? yellow : gray;
        return true;
    }
}
