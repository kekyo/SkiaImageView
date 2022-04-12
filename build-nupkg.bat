@echo off

rem SkiaImageView.Wpf - Easy way showing SkiaSharp-based image objects onto WPF applications.
rem
rem Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
rem
rem Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0

echo.
echo "==========================================================="
echo "Build SkiaImageView.Wpf"
echo.

rem git clean -xfd

dotnet restore
dotnet build -p:Configuration=Release -p:Platform=AnyCPU SkiaImageView.Wpf\SkiaImageView.Wpf.csproj
dotnet pack -p:Configuration=Release -p:Platform=AnyCPU -o artifacts SkiaImageView.Wpf\SkiaImageView.Wpf.csproj
