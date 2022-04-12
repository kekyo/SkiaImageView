# SkiaImageView.Wpf

[![Project Status: WIP – Initial development is in progress, but there has not yet been a stable, usable release suitable for the public.](https://www.repostatus.org/badges/latest/wip.svg)](https://www.repostatus.org/#wip)

## NuGet

|Package|NuGet|
|:--|:--|
|SkiaImageView.Wpf|[![NuGet SkiaImageView.Wpf](https://img.shields.io/nuget/v/SkiaImageView.Wpf.svg?style=flat)](https://www.nuget.org/packages/SkiaImageView.Wpf)|

----

## What is this?

Easy way showing SkiaSharp-based image objects onto WPF applications.

`SKImageView` is a control of SkiaSharp image drawing.
You can manipulate same as with WPF's `Image` control.

Available properties:

|Name|Detail|
|:----|:----|
|`Source`|SkiaSharp image related objects. See listed below.|
|`Stretch`|[Stretch enum value](https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.stretch?view=windowsdesktop-6.0), TODO: currently `None` and `Uniform` are accepted. |
|`StretchDirection`|TODO: [StretchDirection enum value](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.stretchdirection?view=windowsdesktop-6.0)|
|`HorizontalContentAlignment`|TODO: `HorizontalAlignment`|
|`VerticalContentAlignment`|TODO: `VerticalAlignment`|

The `Source` property accepts the following types of objects:

* `SKBitmap`
* `SKImage`
* `SKDrawable`
  * The `SKDrawable` is drawn with the size corresponding to the current `RenderSize` area.

XAML example:

```xml
<Window xmlns:siv="https://github.com/kekyo/SkiaImageView.Wpf">
    <siv:SKImageView
        Stretch="Uniform"
        Source="{Binding PreviewImage}" />
</Window>
```

----

## License

Apache-v2.
