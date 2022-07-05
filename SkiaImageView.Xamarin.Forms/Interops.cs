////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView.Wpf - Easy way showing SkiaSharp-based image objects onto WPF applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using System;
using Xamarin.Forms;

namespace SkiaImageView
{
    internal static class Interops
    {
        public static BindableProperty Register<TTarget, THost>(
            string memberName, TTarget defaultValue, Action<THost, TTarget, TTarget> changed)
            where THost : BindableObject =>
            BindableProperty.Create(
                memberName, typeof(TTarget), typeof(THost),
                defaultValue, BindingMode.OneWay, null, (s, o, n) => changed((THost)s, (TTarget)o, (TTarget)n));

        public static void InvokeAsynchronously(
            this IDispatcher dispatcher, Action action) =>
            dispatcher.BeginInvokeOnMainThread(action);
    }
}
