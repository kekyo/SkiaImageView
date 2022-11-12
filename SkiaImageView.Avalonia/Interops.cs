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
        where THost : IAvaloniaObject =>
        AvaloniaProperty.Register<THost, TTarget>(
            memberName, defaultValue, false, BindingMode.OneWay,
            null,
            null,
            (s, isChanged) =>
            {
                if (isChanged)
                {
                    changed((THost)s);
                }
            });

    public static Dispatcher GetDispatcher(
        this IAvaloniaObject _) =>
        Dispatcher.UIThread;

    public static void InvokeAsynchronously(
        this Dispatcher dispatcher, Action action, bool isHigherPriority) =>
        dispatcher.InvokeAsynchronously(
            action,
            isHigherPriority);
}
