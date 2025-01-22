using Microsoft.Extensions.Caching.Memory;
using MisakaBiliCore.Models.BiliApi;
using MisakaBiliCore.Services.BiliApi;
using MisakaBiliCore.Utils;

namespace MisakaBiliCore.Services;

public class BiliStreamUrlRequestService(
    IBiliApiServices biliApiServices,
    IMemoryCache memoryCache,
    HttpClient httpClient)
{
    private readonly string[] _acceptedHost =
    [
        "upos-sz-mirrorali.bilivideo.com",
        "upos-sz-mirroralib.bilivideo.com",
        "upos-sz-mirroralio1.bilivideo.com",
        "upos-sz-mirrorali02.bilivideo.com",
        "upos-sz-estgoss.bilivideo.com",
        "upos-sz-mirrorcos.bilivideo.com",
        "upos-sz-mirrorcosb.bilivideo.com",
        "upos-sz-mirrorcoso1.bilivideo.com",
        "upos-sz-mirrorcosdisp.bilivideo.com",
        "upos-sz-mirrorhw.bilivideo.com",
        "upos-sz-mirrorhwb.bilivideo.com",
        "upos-sz-mirror08ct.bilivideo.com",
        "upos-sz-mirrorhwo1.bilivideo.com",
        "upos-sz-mirror08c.bilivideo.com",
        "upos-sz-mirror08h.bilivideo.com",
        "upos-sz-mirror08ct.bilivideo.com",
        "upos-sz-mirrorbd.bilivideo.com",
        "upos-sz-upcdnbda2.bilivideo.com",
        "upos-sz-mirrorhwdisp.bilivideo.com"
    ];

    private const string VideoDetailCacheKey = "bili-video-detail-";
    private const string VideoUrlCacheKey = "bili-video-url-";

    private readonly TimeSpan _cacheDuration = TimeSpan.FromMinutes(2);
    private readonly TimeSpan _videoUrlCacheDuration = TimeSpan.FromHours(1);

    public async ValueTask<BiliVideoMp4UrlResponse> GetVideoMp4StreamUrlAsync(string bvidOrAvid, int page,
        BiliVideoQuality quality)
    {
        var videoDetail = await GetVideoDetailInternalAsync(bvidOrAvid);

        if (page > videoDetail.Data.Pages.Length - 1)
            throw new ArgumentOutOfRangeException(nameof(page), "Page out of index");

        var videoBvid = videoDetail.Data.Bvid;
        var cid = videoDetail.Data.Pages[page].Cid;

        var urlResponse = await GetVideoMp4UrlInternalAsync(videoBvid, cid, quality);

        return urlResponse.Data;
    }

    public async ValueTask<BiliVideoDashUrlResponse> GetVideoDashStreamUrlAsync(string bvidOrAvid, int page)
    {
        var videoDetail = await GetVideoDetailInternalAsync(bvidOrAvid);

        if (page > videoDetail.Data.Pages.Length - 1)
            throw new ArgumentOutOfRangeException(nameof(page), "Page out of index");

        var videoBvid = videoDetail.Data.Bvid;
        var cid = videoDetail.Data.Pages[page].Cid;

        return (await GetVideoDashStreamUrlInternalAsync(videoBvid, cid)).Data;
    }

    private async ValueTask<BiliApiResponse<BiliVideoDetail>> GetVideoDetailInternalAsync(string bvidOrAvid)
    {
        if (memoryCache.TryGetValue(VideoDetailCacheKey + bvidOrAvid,
                out BiliApiResponse<BiliVideoDetail>? cacheVideoDetail) &&
            cacheVideoDetail != null)
        {
            return cacheVideoDetail;
        }

        var bvid = bvidOrAvid.StartsWith("BV1") ? bvidOrAvid : BvAvUtils.AvToBv(long.Parse(bvidOrAvid));
        var videoDetail = await biliApiServices.GetVideoDetailByBvid(bvid);

        memoryCache.Set(VideoDetailCacheKey + bvidOrAvid, videoDetail, _cacheDuration);

        return videoDetail;
    }

    private async ValueTask<BiliApiResponse<BiliVideoMp4UrlResponse>> GetVideoMp4UrlInternalAsync(string bvid, long cid,
        BiliVideoQuality quality)
    {
        if (memoryCache.TryGetValue(GetMp4VideoUrlCacheKey(bvid, (int)cid, quality),
                out BiliApiResponse<BiliVideoMp4UrlResponse>? cacheVideoUrl) &&
            cacheVideoUrl != null)
        {
            return cacheVideoUrl;
        }

        var videoUrlResponse = await biliApiServices.GetVideoMp4UrlByBvid(bvid, cid, (int)quality);

        videoUrlResponse = videoUrlResponse with
        {
            Data = videoUrlResponse.Data with
            {
                Durl = await FilterVideoUrlItemsAsync(videoUrlResponse.Data.Durl)
            }
        };

        memoryCache.Set(GetMp4VideoUrlCacheKey(bvid, (int)cid, quality), videoUrlResponse, _videoUrlCacheDuration);

        return videoUrlResponse;
    }

    private async ValueTask<BiliApiResponse<BiliVideoDashUrlResponse>> GetVideoDashStreamUrlInternalAsync(string bvid,
        long cid)
    {
        if (memoryCache.TryGetValue(GetDashVideoUrlCacheKey(bvid, (int)cid),
                out BiliApiResponse<BiliVideoDashUrlResponse>? cacheVideoDash) &&
            cacheVideoDash != null)
        {
            return cacheVideoDash;
        }

        var dashResponse = await biliApiServices.GetVideoDashUrlByBvid(bvid, cid);

        dashResponse = dashResponse with
        {
            Data = dashResponse.Data with
            {
                Dash = dashResponse.Data.Dash with
                {
                    Audio = await FilterVideoUrlItemsAsync(dashResponse.Data.Dash.Audio),
                    Video = await FilterVideoUrlItemsAsync(dashResponse.Data.Dash.Video)
                }
            }
        };

        memoryCache.Set(GetDashVideoUrlCacheKey(bvid, (int)cid), dashResponse, _videoUrlCacheDuration);

        return dashResponse;
    }

    #region Process Video Items

    private async ValueTask<T[]> FilterVideoUrlItemsAsync<T>(T[] videoItems) where T : BiliVideoUrlItemBase
    {
        var urlItems = videoItems.Select(item => new
            {
                Item = item,
                Urls = (Uri[]) [new Uri(item.Url), ..(item.BackupUrls ?? []).Select(url => new Uri(url))]
            })
            .ToArray();

        // Steps to filter video stream URL items:
        // 1. If any video stream URL has a host in the accepted host list, filter and return those URLs.
        // 2. If not, sort items (non-P2P URLs first), exclude URLs with "/v1/" in its path,
        //    replace their host with an accepted host, and send an HTTP HEAD request to test validity.
        //    If valid, return those items.
        // 3. If step 2 fails, return non-P2P URLs if they exist.
        // 4. If no non-P2P URLs exist, return all items.

        // Step 1: Filter URLs with accepted hosts
        var acceptedItems = urlItems.Where(item => item.Urls.Any(url => _acceptedHost.Contains(url.Host)))
            .SelectMany(item =>
            {
                var acceptedUrls = item.Urls
                    .Where(url => _acceptedHost.Contains(url.Host))
                    .Select(url => url.ToString())
                    .ToArray();

                return TransformVideoUrlItem(acceptedUrls, item.Item);
            })
            .ToArray();

        if (acceptedItems.Length > 0)
        {
            return acceptedItems;
        }

        // Step 2: Sort items and test validity
        var noP2PRegex = NoP2PUtils.P2PRegex();
        var noP2PItems = urlItems
            .SelectMany(item =>
            {
                var nonP2PUrls = item.Urls
                    .Where(url => !noP2PRegex.IsMatch(url.Host))
                    .Select(url => url.ToString())
                    .ToArray();

                return TransformVideoUrlItem(nonP2PUrls, item.Item);
            })
            .ToArray();

        var p2pItems = urlItems
            .SelectMany(item =>
            {
                var p2PUrls = item.Urls
                    .Where(url => noP2PRegex.IsMatch(url.Host))
                    .Select(url => url.ToString())
                    .ToArray();

                return TransformVideoUrlItem(p2PUrls, item.Item);
            })
            .ToArray();

        var sortedItems = noP2PItems.Concat(p2pItems).ToArray();

        var itemsReplacedUrl = sortedItems.SelectMany(item =>
            {
                List<T> items = [];

                foreach (var acceptHost in _acceptedHost)
                {
                    var urlBuilder = new UriBuilder(item.Url)
                    {
                        Host = acceptHost
                    };

                    items.Add(item with
                    {
                        Url = urlBuilder.Uri.ToString()
                    });
                }

                return items;
            })
            .ToArray();

        var validResults = await ValidUrlItemsAsync(itemsReplacedUrl);
        if (validResults.Length > 0)
        {
            return validResults;
        }

        // Step 3: Return non-P2P URLs if they exist or Step 4: Return all items
        return noP2PItems.Length > 0 ? noP2PItems : sortedItems;
    }

    private async ValueTask<T[]> ValidUrlItemsAsync<T>(T[] items) where T : BiliVideoUrlItemBase
    {
        var validTasks = items.Select(item => Task.Run(async () =>
            {
                try
                {
                    var response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, item.Url),
                        HttpCompletionOption.ResponseHeadersRead,
                        new CancellationTokenSource(TimeSpan.FromSeconds(5)).Token);
                    response.EnsureSuccessStatusCode();

                    return item;
                }
                catch
                {
                    // ignored
                }

                return null;
            }))
            .ToArray();

        await Task.WhenAll(validTasks);

        var taskResults = validTasks.Select(task => task.Result)
            .Where(item => item != null)
            .OfType<T>()
            .ToArray();

        return taskResults;
    }

    private static T[] TransformVideoUrlItem<T>(string[] urls, T item) where T : BiliVideoUrlItemBase
    {
        return urls.Select(url => item with { Url = url }).ToArray();
    }

    #endregion

    private static string GetMp4VideoUrlCacheKey(string bvid, int page, BiliVideoQuality quality)
    {
        return VideoUrlCacheKey + "mp4-" + bvid + "-" + page + "-" + quality;
    }

    private static string GetDashVideoUrlCacheKey(string bvid, int page)
    {
        return VideoUrlCacheKey + "dash-" + bvid + "-" + page;
    }
}
