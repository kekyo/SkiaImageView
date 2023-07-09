@echo off

rem SkiaImageView - Easy way showing SkiaSharp-based image objects onto UI applications.
rem
rem Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
rem
rem Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0

echo.
echo "==========================================================="
echo "Build SkiaImageView"
echo.

rem git clean -xfd

dotnet build -p:Configuration=Release -p:Platform=AnyCPU SkiaImageView.Wpf\SkiaImageView.Wpf.csproj
dotnet build -p:Configuration=Release -p:Platform=AnyCPU SkiaImageView.Xamarin.Forms\SkiaImageView.Xamarin.Forms.csproj
dotnet build -p:Configuration=Release -p:Platform=AnyCPU SkiaImageView.Maui\SkiaImageView.Maui.csproj
dotnet build -p:Configuration=Release -p:Platform=AnyCPU SkiaImageView.Avalonia11\SkiaImageView.Avalonia11.csproj
dotnet build -p:Configuration=Release -p:Platform=AnyCPU SkiaImageView.Avalonia\SkiaImageView.Avalonia.csproj

dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -o artifacts SkiaImageView.Wpf\SkiaImageView.Wpf.csproj
dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -o artifacts SkiaImageView.Xamarin.Forms\SkiaImageView.Xamarin.Forms.csproj
dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -o artifacts SkiaImageView.Maui\SkiaImageView.Maui.csproj
dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -o artifacts SkiaImageView.Avalonia11\SkiaImageView.Avalonia11.csproj
dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -o artifacts SkiaImageView.Avalonia\SkiaImageView.Avalonia.csproj
