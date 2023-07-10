# SkiaImageView

![SkiaImageView](Images/SkiaImageView.100.png)

[![Project Status: Active – The project has reached a stable, usable state and is being actively developed.](https://www.repostatus.org/badges/latest/active.svg)](https://www.repostatus.org/#active)

## NuGet

|Package|NuGet|
|:----|:----|
|SkiaImageView.Wpf|[![NuGet SkiaImageView.Wpf](https://img.shields.io/nuget/v/SkiaImageView.Wpf.svg?style=flat)](https://www.nuget.org/packages/SkiaImageView.Wpf)|
|SkiaImageView.Avalonia11|[![NuGet SkiaImageView.Avalonia11](https://img.shields.io/nuget/v/SkiaImageView.Avalonia11.svg?style=flat)](https://www.nuget.org/packages/SkiaImageView.Avalonia11)|
|SkiaImageView.Avalonia|[![NuGet SkiaImageView.Avalonia](https://img.shields.io/nuget/v/SkiaImageView.Avalonia.svg?style=flat)](https://www.nuget.org/packages/SkiaImageView.Avalonia)|
|SkiaImageView.Xamarin.Forms|[![NuGet SkiaImageView.Xamarin.Forms](https://img.shields.io/nuget/v/SkiaImageView.Xamarin.Forms.svg?style=flat)](https://www.nuget.org/packages/SkiaImageView.Xamarin.Forms)|
|SkiaImageView.Maui|[![NuGet SkiaImageView.Maui](https://img.shields.io/nuget/v/SkiaImageView.Maui.svg?style=flat)](https://www.nuget.org/packages/SkiaImageView.Maui)|

----

[English language](https://github.com/kekyo/SkiaImageView/)

## これは何?

[SkiaSharp](https://github.com/mono/SkiaSharp) ベースのイメージオブジェクトを、UIアプリケーションに簡単に表示するためのコントロールです。
`Source` プロパティに、SkiaSharpのイメージオブジェクトをバインディングするだけで表示できます。

`SKImageView` コントロールは、[WPFの `System.Windows.Controls.Image`](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.image?view=windowsdesktop-6.0)
とほぼ同じ使い方で表示させることが出来ます。

SkiaSharpイメージオブジェクトとしてサポートしているクラスです: `SKBitmap`, `SKImage`, `SKPicture`, `SKDrawable`, `SKSurface`

XAMLの例:

```xml
<Window xmlns:siv="https://github.com/kekyo/SkiaImageView">
    <siv:SKImageView
        Stretch="Uniform"
        Source="{Binding PreviewImage}" />
</Window>
```

```csharp
public sealed class ViewModel
{
    public SKBitmap? PreviewImage { get; set; }

    // ...
}
```

サンプルコードの一覧です:

* [Avalonia 11サンプル](https://github.com/kekyo/SkiaImageView/tree/main/samples/SkiaImageView.Avalonia11.Sample)
* [Avalonia (older)サンプル](https://github.com/kekyo/SkiaImageView/tree/main/samples/SkiaImageView.Avalonia.Sample)
* [WPFサンプル](https://github.com/kekyo/SkiaImageView/tree/main/samples/SkiaImageView.Wpf.Sample)
* [Xamarin Formsサンプル](https://github.com/kekyo/SkiaImageView/tree/main/samples/SkiaImageView.Xamarin.Forms.Sample)
* [.NET MAUIサンプル](https://github.com/kekyo/SkiaImageView/tree/main/samples/SkiaImageView.Maui.Sample)

どのプラットフォームで使用しても、使いかたはほぼ同一です。

----

## サポートされているプラットフォーム

### WPF

* .NET 7, 6, 5 (`net7.0-windows`, `net6.0-windows`, `net5.0-windows`)
* .NET Core 3.1, 3.0 (`netcoreapp3.1`, `netcoreapp3.0`)
* .NET Framework 4.8, 4.6.2 (`net48`, `net462`)
* SkiaSharp: 2.80.0以上

### Avalonia

* .NET 7, 6, 5 (`net7.0`, `net6.0`, `net5.0`)
* .NET Core 3.1, 3.0, (`netcoreapp3.1`, `netcoreapp3.0`)
* .NET Core 2.2, 2.1, 2.0 (`netcoreapp2.2`, `netcoreapp2.1`, `netcoreapp2.0`)
* .NET Framework 4.8, 4.6.2 (`net48`, `net462`)
* Avalonia 11:
  * Avalonia: 11.0.0以上
  * SkiaSharp: 2.88.3以上
* Avalonia (older):
  * Avalonia: 0.10.0以上
  * SkiaSharp: 2.80.0以上
  * 
### Xamarin Forms

* .NET Standard 2.0 (`netstandard2.0`)
* Xamarin Forms: 5.0.0.1874以上
* SkiaSharp: 2.80.0以上

### .NET MAUI

* .NET 7, 6 (`net7.0`, `net6.0`)
* .NET MAUI: 6以上
* SkiaSharp: 2.88.0以上 (2.80.0ではありません。何故なら、このバージョンには不正な型参照が含まれており、MAUI環境では正しく機能しないためです。)

----

## プロパティ一覧

|プロパティ名|内容|
|:----|:----|
|`Source`|SkiaSharpオブジェクトを設定します。一覧は後述。|
|`Stretch`|[Stretch列挙型](https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.stretch?view=windowsdesktop-6.0)|
|`StretchDirection`|[StretchDirection列挙型](https://docs.microsoft.com/en-us/dotnet/api/system.windows.controls.stretchdirection?view=windowsdesktop-6.0)|
|`RenderMode`|レンダリングモードを示す列挙型|

### Source property

`Source`プロパティには、以下のSkiaSharpオブジェクトを設定出来ます:

|使用可能な型|アスペクト比の計算元|備考|
|:----|:----|:----|
|`SKBitmap`|元のイメージ| |
|`SKImage`|元のイメージ| |
|`SKPicture`|配置先の`RenderSize`による| |
|`SKDrawable`|配置先の`RenderSize`による| |
|`SKSurface`|配置先の`RenderSize`による| |
|`string`|元のイメージ|イメージをダウンロードするURL|
|`Uri`|元のイメージ|イメージをダウンロードするURL|

いくつかの型は描画時のアスペクト比を、コントロールが配置された場所の `RenderSize` によって決定します。
その場合、イメージの正しいアスペクト比を維持するためには、XAMLによって制御する必要があります。

注意: WPFにおいてURLを指定する場合は、WPFのリソースフォームURLを認識できません。
(これらは、`application:` や `pack:` から始まります。)

### RenderModeプロパティ

イメージのレンダリングを行う方法を、以下から選択出来ます:

|RenderMode|詳細|
|:----|:----|
|`Synchronously`|全てのレンダリングは、同期的に実行されます。|
|`AsynchronouslyForFetching`|デフォルトです。`Source`プロパティがURLで指定された場合のみ、イメージのフェッチを非同期で実行し、その後表示します。|
|`Asynchronously`|全てのレンダリングは、非同期的に実行されます。|

`AsynchronouslyForFetching`がデフォルトです。
`Asynchronously`を使用する場合は、`Source`に指定されるイメージの詳細が、暗黙の内に変更されないようにする必要があります。
この制約を実現するのは難易度が高いかもしれません。

----

## ライセンス

Apache-v2.

----

## 履歴

[英語のREADME](https://github.com/kekyo/SkiaImageView#history)を参照して下さい。
