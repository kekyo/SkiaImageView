////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView - Easy way showing SkiaSharp-based image objects onto UI applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SkiaImageView.Sample.Models;

public static class TheCatAPI
{
    private static readonly HttpClient httpClient = new HttpClient();

    public static async ValueTask<Cat[]> FetchTheCatsAsync(int cats)
    {
        // Uses The Cat API (https://thecatapi.com/)
        using (var response =
            await httpClient.
                GetAsync($"https://api.thecatapi.com/v1/images/search?limit={cats}").
                ConfigureAwait(false))
        {
            using (var stream =
                await response.Content.ReadAsStreamAsync().
                    ConfigureAwait(false))
            {
                var tr = new StreamReader(stream, Encoding.UTF8, true);
                var jr = new JsonTextReader(tr);

                var serializer = new JsonSerializer();

                return serializer.Deserialize<Cat[]>(jr)!;
            }
        }
    }

    public static async ValueTask<byte[]> FetchImageAsync(Uri url)
    {
        using (var response =
            await httpClient.
                GetAsync(url).
                ConfigureAwait(false))
        {
            using (var stream =
                await response.Content.ReadAsStreamAsync().
                    ConfigureAwait(false))
            {
                var ms = new MemoryStream();
                await stream.CopyToAsync(ms).
                    ConfigureAwait(false);

                return ms.ToArray();
            }
        }
    }
}
