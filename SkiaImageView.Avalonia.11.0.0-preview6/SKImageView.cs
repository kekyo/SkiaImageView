////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView - Easy way showing SkiaSharp-based image objects onto UI applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Metadata;
using SkiaSharp;

namespace SkiaImageView;

public partial class SKImageView : Control
{
    public static readonly AvaloniaProperty<SKObject?> SourceProperty =
        AvaloniaProperty.Register<SKImageView, SKObject?>(
            nameof(Source),null,coerce:(sender,e)=>
            {
                if(sender is SKImageView control)
                {
                    if (e is SKBitmap bitmap)
                    {
                        control.SourceSize = new Size(bitmap.Width, bitmap.Height);
                    }
                    else if (e is SKImage image)
                    {
                        control.SourceSize = new Size(image.Width, image.Height);
                    }
                    else
                    {
                        control.SourceSize = control.RenderSize;
                    }
                }
                return e;
            });

    public static readonly AvaloniaProperty<Stretch> StretchProperty =
        AvaloniaProperty.Register<SKImageView, Stretch>(
            nameof(Stretch), Stretch.None);

    public static readonly AvaloniaProperty<StretchDirection> StretchDirectionProperty =
        AvaloniaProperty.Register<SKImageView, StretchDirection>(
            nameof(StretchDirection), StretchDirection.Both);

    public Stretch Stretch
    {
        get => (Stretch)this.GetValue(StretchProperty)!;
        set => this.SetValue(StretchProperty, value);
    }

    public StretchDirection StretchDirection
    {
        get => (StretchDirection)this.GetValue(StretchDirectionProperty)!;
        set => this.SetValue(StretchDirectionProperty, value);
    }

    private readonly object _sync = new();

    static SKImageView()
    {
        AffectsRender<SKImageView>(SourceProperty, StretchProperty, StretchDirectionProperty);
        AffectsMeasure<SKImageView>(SourceProperty, StretchProperty, StretchDirectionProperty);
        AffectsArrange<SKImageView>(SourceProperty, StretchProperty, StretchDirectionProperty);
    }

    [Content]
    public SKObject? Source
    {
        get => (SKObject?)this.GetValue(SourceProperty);
        set => this.SetValue(SourceProperty, value);
    }

    private Size RenderSize => this.Bounds.Size;

    private Size SourceSize { set; get; }

    protected override Size MeasureOverride(Size constraint)
    {
        if (Source is { } source)
        {
            return Stretch.CalculateSize(constraint, SourceSize, StretchDirection);
        }
        else
        {
            return default;
        }
    }

    protected override Size ArrangeOverride(Size arrangeSize)
    {
        if (Source is { } source)
        {
            return Stretch.CalculateSize(arrangeSize, SourceSize);
        }
        else
        {
            return default;
        }
    }

    public override void Render(DrawingContext drawingContext)
    {
        if(Source is { } source)
        {
            Size sourceSize = SourceSize;

            var viewPort = new Rect(RenderSize);
            var scale = Stretch.CalculateScaling(RenderSize, sourceSize, StretchDirection);
            var scaledSize = sourceSize * scale;
            var destRect = viewPort
                .CenterRect(new Rect(scaledSize))
                .Intersect(viewPort);
            var sourceRect = new Rect(sourceSize)
                .CenterRect(new Rect(destRect.Size / scale));

            //var bounds = SKRect.Create(new SKPoint(), new SKSize { Height = (float)SKRect.Create(new SKPoint(), new SKSize { Height = (float)sourceSize.Height, Width = (float)sourceSize.Width }).Height, Width = (float)sourceSize.Width });
            var bounds = SKRect.Create(0f, 0f, (float)sourceSize.Width, (float)sourceSize.Height);
            var scaleMatrix = Matrix.CreateScale(
                destRect.Width / sourceRect.Width,
                destRect.Height / sourceRect.Height);
            var translateMatrix = Matrix.CreateTranslation(
                -sourceRect.X + destRect.X - bounds.Top,
                -sourceRect.Y + destRect.Y - bounds.Left);

            using (drawingContext.PushClip(destRect))
            using (drawingContext.PushTransform(translateMatrix * scaleMatrix))
            {
                drawingContext.Custom(new FuncCustomDrawOperation(bounds, Draw));
            }

        }
        else
        {
            base.Render(drawingContext);
        }
    }

    private void Draw(SKCanvas canvas,SKRect rect)
    {
        lock (_sync)
        {
            canvas.Save();

            if (Source is SKBitmap bitmap)
            {
                canvas.DrawBitmap(bitmap, rect, default);
            }
            else if (Source is SKImage image)
            {
                canvas.DrawImage(image, rect, default);
            }
            else if (Source is SKPicture picture)
            {
                canvas.DrawPicture(picture, default);
            }
            else if (Source is SKDrawable drawable)
            {
                canvas.DrawDrawable(drawable, default);
            }
            else if (Source is SKSurface surface)
            {
                canvas.DrawSurface(surface, default);
            }
            canvas.Restore();
        }
    }
}
