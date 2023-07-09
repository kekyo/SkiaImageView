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
using Avalonia.Data;
using Avalonia.Reactive;
using Avalonia.Threading;
using System;

namespace SkiaImageView;

internal static class Interops
{
#if AVALONIA
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
#else

// The same AvaloniaProperty should not be registered twice
#pragma warning disable AVP1001

    public static AvaloniaProperty<TTarget> Register<TTarget, THost>(
        string memberName, TTarget defaultValue, Action<THost> changed)
        where THost : AvaloniaObject
    {
        var ap = AvaloniaProperty.Register<THost, TTarget>(
            memberName, defaultValue, false, BindingMode.OneWay,
            null,
            null,
            false);

        ap.Changed.Subscribe(new AnonymousObserver<AvaloniaPropertyChangedEventArgs<TTarget>>(
            e => changed((THost)e.Sender)));

        return ap;
    }

    public static Dispatcher GetDispatcher(
        this AvaloniaObject _) =>
        Dispatcher.UIThread;
#endif

    public static void InvokeAsynchronously(
        this Dispatcher dispatcher, Action action, bool isHigherPriority) =>
        dispatcher.InvokeAsynchronously(
            action,
            isHigherPriority);
}
