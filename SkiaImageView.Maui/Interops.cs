////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView - Easy way showing SkiaSharp-based image objects onto UI applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Microsoft.Maui.Controls;
using Microsoft.Maui.Dispatching;
using System;

namespace SkiaImageView;

internal static class Interops
{
    public static BindableProperty Register<TTarget, THost>(
        string memberName, TTarget defaultValue, Action<THost> changed)
        where THost : BindableObject =>
        BindableProperty.Create(
            memberName, typeof(TTarget), typeof(THost),
            defaultValue, BindingMode.OneWay, null,
            (s, _, _) => changed((THost)s));

    public static IDispatcher GetDispatcher(
        this BindableObject b) =>
        b.Dispatcher;

    public static void InvokeAsynchronously(
        this IDispatcher dispatcher, Action action, bool isHigherPriority) =>
        dispatcher.InvokeAsynchronously(action, isHigherPriority);
}
