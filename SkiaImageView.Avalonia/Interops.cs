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
using Avalonia.Data;
using Avalonia.Layout;
using Avalonia.Threading;
using System;

namespace SkiaImageView;

internal static class Interops
{
    public static AvaloniaProperty<TTarget> Register<TTarget, THost>(
        string memberName, TTarget defaultValue, Action<THost> changed)
        where THost : AvaloniaObject =>
        AvaloniaProperty.Register<THost, TTarget>(
            memberName, defaultValue, false, BindingMode.OneWay,
            null,
            (sender, newValue) => { changed((THost)sender); return newValue; },
            false);

    public static Dispatcher GetDispatcher(
        this AvaloniaObject _) =>
        Dispatcher.UIThread;

    public static void InvokeAsynchronously(
        this Dispatcher dispatcher, Action action, bool isHigherPriority) =>
        dispatcher.InvokeAsynchronously(
            action,
            isHigherPriority);
}
