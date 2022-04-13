# SkiaImageView.Wpf

![SkiaImageView.Wpf](Images/SkiaImageView.Wpf.100.png)

[![Project Status: WIP – Initial development is in progress, but there has not yet been a stable, usable release suitable for the public.](https://www.repostatus.org/badges/latest/wip.svg)](https://www.repostatus.org/#wip)

## NuGet

|Package|NuGet|
|:--|:--|
|SkiaImageView.Wpf|[![NuGet SkiaImageView.Wpf](https://img.shields.io/nuget/v/SkiaImageView.Wpf.svg?style=flat)](https://www.nuget.org/packages/SkiaImageView.Wpf)|

----

## What is this?

Easy way showing [SkiaSharp](https://github.com/mono/SkiaSharp)-based image objects onto WPF applications.

`SKImageView` is a control of SkiaSharp image drawing.
You can manipulate same as with [WPF's `System.Windows.Controls.Image`](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.image?view=windowsdesktop-6.0).

XAML example:

```xml
<Window xmlns:siv="https://github.com/kekyo/SkiaImageView.Wpf">
    <siv:SKImageView
        Stretch="Uniform"
        Source="{Binding PreviewImage}" />
</Window>
```

----

## Supported platform

* .NET 6.0, 5.0 (`net6.0-windows`, `net5.0-windows`)
* .NET Core 3.1, 3.0 (`netcoreapp3.1`, `netcoreapp3.0`)
* .NET Framework 4.8, 4.6.2 (`net48`, `net462`)
* SkiaSharp: 2.80.3 or upper.

----

## Available properties

|Name|Detail|
|:----|:----|
|`Source`|SkiaSharp image related objects. See listed below.|
|`Stretch`|[Stretch enum value](https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.stretch?view=windowsdesktop-6.0), TODO: currently `UniformToFill` is not supported. |
|`StretchDirection`|TODO: [StretchDirection enum value](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.stretchdirection?view=windowsdesktop-6.0)|

The `Source` property accepts the following SkiaSharp types:

|Type|Aspect ratio from|
|:----|:----|
|`SKBitmap`|Origin|
|`SKImage`|Origin|
|`SKPicture`|Measured `RenderSize`|
|`SKDrawable`|Measured `RenderSize`|
|`SKSurface`|Measured `RenderSize`|

Some types are drawn with aspect ratio corresponding to the current measured `RenderSize` area.
Therefore, to maintain the aspect ratio, the size must be explicitly controlled in XAML.

----

## License

Apache-v2.
