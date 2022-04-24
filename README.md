# SkiaImageView.Wpf

![SkiaImageView.Wpf](Images/SkiaImageView.Wpf.100.png)

[![Project Status: Active – The project has reached a stable, usable state and is being actively developed.](https://www.repostatus.org/badges/latest/active.svg)](https://www.repostatus.org/#active)

## NuGet

|Package|NuGet|
|:--|:--|
|SkiaImageView.Wpf|[![NuGet SkiaImageView.Wpf](https://img.shields.io/nuget/v/SkiaImageView.Wpf.svg?style=flat)](https://www.nuget.org/packages/SkiaImageView.Wpf)|

----

## What is this?

Easy way showing [SkiaSharp](https://github.com/mono/SkiaSharp)-based image objects onto WPF applications.

`SKImageView` is a control of SkiaSharp image drawing.
You can manipulate same as with [WPF's `System.Windows.Controls.Image`](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.image?view=windowsdesktop-6.0).

Supported SkiaSharp types are: `SKBitmap`, `SKImage`, `SKPicture`, `SKDrawable` and `SKSurface`.

XAML example:

```xml
<Window xmlns:siv="https://github.com/kekyo/SkiaImageView.Wpf">
    <siv:SKImageView
        Stretch="Uniform"
        Source="{Binding PreviewImage}" />
</Window>
```

Fully sample code is here: [SkiaImageView.Wpf.Sample](https://github.com/kekyo/SkiaImageView.Wpf/tree/main/samples/SkiaImageView.Wpf.Sample)

----

## Supported platform

* .NET 6.0, 5.0 (`net6.0-windows`, `net5.0-windows`)
* .NET Core 3.1, 3.0 (`netcoreapp3.1`, `netcoreapp3.0`)
* .NET Framework 4.8, 4.6.2 (`net48`, `net462`)
* SkiaSharp: 2.80.0 or upper.

----

## Available properties

|Name|Detail|
|:----|:----|
|`Source`|SkiaSharp image related objects. See listed below.|
|`Stretch`|[Stretch enum value](https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.stretch?view=windowsdesktop-6.0)|
|`StretchDirection`|[StretchDirection enum value](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.stretchdirection?view=windowsdesktop-6.0)|
|`RenderMode`|Rendering into back buffer by synchronous or asynchronous.|

### Source property

The `Source` property accepts the following SkiaSharp types:

|Supported Type|Aspect ratio from|Note|
|:----|:----|:----|
|`SKBitmap`|Origin| |
|`SKImage`|Origin| |
|`SKPicture`|Measured `RenderSize`| |
|`SKDrawable`|Measured `RenderSize`| |
|`SKSurface`|Measured `RenderSize`| |
|`string`|Origin|URL string for downloading content|
|`Uri`|Origin|URL for downloading content|

Some types are drawn with aspect ratio corresponding to the current measured `RenderSize` area.
Therefore, to maintain the aspect ratio, the size must be explicitly controlled in XAML.

Note: If you specify a URL to display, the URL does NOT accept the WPF resource format.
(`application:` and `pack:` protocol based.)

### RenderMode property

Choose rendering into back buffer by synchronous or asynchronous:

|RenderMode|Note|
|:----|:----|
|`Synchronously`|All rendering process is synchronously.|
|`AsynchronouslyForFetching`|Defaulted, Will operate asynchronously when giving URL in `Source` property (`string` or `Uri`).|
|`Asynchronously`|All rendering process is asynchronously.|

`AsynchronouslyForFetching` is defaulted.
Because, when set to `Asynchronously`, all instances given to `Source` must not be implicitly modified.
Maybe, this constraint can be difficult to achieve on your project.

----

## License

Apache-v2.

----

## History

* 1.0.1:
  * Downgraded SkiaSharp to 2.80.0 (Because known bug related.)
* 1.0.0:
  * Reached 1.0.0 🎉
  * Fixed updating new image instance.
* 0.4.0:
  * Added RenderMode features and supported StretchDirection.
  * Added sample code.
* 0.3.0:
  * Fixed XAML namespace.
* 0.2.0:
  * Fixed some problems, add WIP feature.
* 0.1.0:
  * Initial release.
