
using Avalonia.Threading;
using ReactiveUI.Fody.Helpers;
using SkiaSharp;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace SkiaImageView.Avalonia.Sample.ViewModels
{
    public class MainWindowViewModel: ViewModelBase
    {
        public string Greeting => "Welcome to Avalonia!";

        /// <summary>
        /// 实时画面
        /// </summary>
        [Reactive]
        public SKObject? Image { get; private set; }

        private CancellationTokenSource? _cancellationTokenSource;

        public void Start()
        {
            if(_cancellationTokenSource is { })
            {
                _cancellationTokenSource.Cancel();
                _cancellationTokenSource = null;
                return;
            }

            _cancellationTokenSource =new CancellationTokenSource();

            LoadImage(_cancellationTokenSource.Token);
        }

        private void LoadImage(CancellationToken token)
        {
            images.Clear();
            Task.Run(() =>
            {
                if (Directory.Exists("Images"))
                {
                    var files = new DirectoryInfo("Images").GetFiles("*.jpeg");

                    // Ready images
                    foreach (var file in files)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            try
                            {
                                var buffer = File.ReadAllBytes(file.FullName);
                                images.Add(SKImage.FromEncodedData(buffer));
                                //images.Add(SKBitmap.Decode(buffer));
                            }
                            catch
                            {
                            }
                        }
                        else
                        {
                            break;
                        }
                    }

                }

                if(images.Count ==0)
                {
                    return;
                }
                while (!token.IsCancellationRequested)
                {
                    foreach (var image in images)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            Dispatcher.UIThread.Post(() =>
                            {
                                Image = image;
                            }, DispatcherPriority.MaxValue);

                            Task.Delay(1).Wait();
                        }
                        else
                        {
                            break;
                        }
                    }
                }
            }, token);
        }

        private List<SKObject> images =new List<SKObject>();
    }
}