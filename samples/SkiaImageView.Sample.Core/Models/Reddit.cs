////////////////////////////////////////////////////////////////////////////
//
// SkiaImageView.Wpf - Easy way showing SkiaSharp-based image objects onto WPF applications.
//
// Copyright (c) Kouji Matsui (@kozy_kekyo, @kekyo@mastodon.cloud)
//
// Licensed under Apache-v2: https://opensource.org/licenses/Apache-2.0
//
////////////////////////////////////////////////////////////////////////////

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace SkiaImageView.Sample.Models
{
    public static class Reddit
    {
        private static readonly HttpClient httpClient = new HttpClient();

        public static async ValueTask<RedditPost[]> FetchNewPostsAsync(string name)
        {
            // Uses Reddit with Json API
            using (var response =
                await httpClient.
                    GetAsync($"https://www.reddit.com/{name}/new.json").
                    ConfigureAwait(false))
            {
                using (var stream =
                    await response.Content.ReadAsStreamAsync().
                        ConfigureAwait(false))
                {
                    var tr = new StreamReader(stream, Encoding.UTF8, true);
                    var jr = new JsonTextReader(tr);

                    var serializer = new JsonSerializer();

                    var root = serializer.Deserialize<JObject>(jr);

                    return root!["data"]!["children"]!.
                        Select(child => child["data"]!).
                        Where(data => Path.GetExtension(((Uri)data["url"]!).AbsolutePath) switch { ".jpg" => true, ".png" => true, _ => false }).
                        Select(data => new RedditPost((string)data["title"]!, (Uri)data["url"]!, (int)data["score"]!)).
                        ToArray();
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
}
