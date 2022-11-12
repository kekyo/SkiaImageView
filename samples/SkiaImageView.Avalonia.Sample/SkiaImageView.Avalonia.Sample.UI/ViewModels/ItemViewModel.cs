////////////////////////////////////////////////////////////////////////////
//
// Epoxy template source code.
// Write your own copyright and note.
// (You can use https://github.com/rubicon-oss/LicenseHeaderManager)
//
////////////////////////////////////////////////////////////////////////////

using Epoxy;
using SkiaSharp;

namespace SkiaImageView.Sample.ViewModels
{
    [ViewModel]
    public sealed class ItemViewModel
    {
        public string? Title { get; set; }

        public SKBitmap? Image { get; set; }

        public int Score { get; set; }
    }
}
