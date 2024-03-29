﻿////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView - Easy way showing SkiaSharp-based image objects onto UI applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using System.Windows;
using System.Windows.Threading;

namespace SkiaImageView;

internal static class Interops
{
    public static DependencyProperty Register<TTarget, THost>(
        string memberName, TTarget defaultValue, Action<THost> changed)
        where THost : DependencyObject =>
        DependencyProperty.Register(
            memberName, typeof(TTarget), typeof(THost),
            new PropertyMetadata(
                defaultValue,
                (s, _) => changed((THost)s)));

    public static Dispatcher GetDispatcher(
        this DependencyObject d) =>
        d.Dispatcher;

    public static void InvokeAsynchronously(
        this Dispatcher dispatcher, Action action, bool isHigherPriority) =>
        dispatcher.BeginInvoke(
            action,
            isHigherPriority ? DispatcherPriority.Normal : DispatcherPriority.ApplicationIdle);
}
