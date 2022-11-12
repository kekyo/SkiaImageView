////////////////////////////////////////////////////////////////////////////
//
// Epoxy template source code.
// Write your own copyright and note.
// (You can use https://github.com/rubicon-oss/LicenseHeaderManager)
//
////////////////////////////////////////////////////////////////////////////

using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace SkiaImageView.Sample.Views
{
    public sealed class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void InitializeComponent() =>
            AvaloniaXamlLoader.Load(this);
    }
}
