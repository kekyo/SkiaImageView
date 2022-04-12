# SkiaImageView.Wpf

[![Project Status: WIP – Initial development is in progress, but there has not yet been a stable, usable release suitable for the public.](https://www.repostatus.org/badges/latest/wip.svg)](https://www.repostatus.org/#wip)

## NuGet

|Package|NuGet|
|:--|:--|
|SkiaImageView.Wpf|[![NuGet SkiaImageView.Wpf](https://img.shields.io/nuget/v/SkiaImageView.Wpf.svg?style=flat)](https://www.nuget.org/packages/SkiaImageView.Wpf)|

----

## What is this?

Easy way showing SkiaSharp-based image objects onto WPF applications.

The `Image` property accepts the following types of objects:

* `SKBitmap`
* `SKImage`
* `SKDrawable`
  * The `SKDrawable` is drawn with the size corresponding to the current `RenderSize` area.

XAML example:

```xml
<Window xmlns:siv="https://github.com/kekyo/SkiaImageView.Wpf">
    <siv:SKImageView
        Fitting="AspectFit"
        Image="{Binding PreviewImage}" />
</Window>
```

----

## License

Apache-v2.
