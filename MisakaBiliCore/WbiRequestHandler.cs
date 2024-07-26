using System.Net.Http.Json;
using MisakaBiliCore.Models.BiliApi;
using MisakaBiliCore.Services.BiliApi;
using MisakaBiliCore.Utils;

namespace MisakaBiliCore;

public class WbiRequestHandler(IHttpClientFactory httpClientFactory) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var httpClient = httpClientFactory.CreateClient("biliapi");

        if (request.RequestUri.LocalPath.Contains("/wbi/"))
        {
            var dateTime = TimeProvider.System.GetUtcNow();
            var queryStrings = UrlUtils.GetQueryStringCollection(request.RequestUri.Query);

            var navMenu =
                await httpClient.GetFromJsonAsync<BiliApiResponse<BiliNavMenuResponse>>("/x/web-interface/nav",
                    cancellationToken: cancellationToken);

            var imgKey = Path.GetFileNameWithoutExtension(new Uri(navMenu.Data.WbiImage.ImgUrl).AbsolutePath);
            var subKey = Path.GetFileNameWithoutExtension(new Uri(navMenu.Data.WbiImage.SubUrl).AbsolutePath);

            var mixinKey = WbiUtils.GetMixinKey(imgKey + subKey);
            var wrid = await WbiUtils.GetWRidAsync(queryStrings, mixinKey, dateTime);

            queryStrings.Add("w_rid", wrid);
            queryStrings.Add("wts", dateTime.ToUnixTimeSeconds().ToString());

            request.RequestUri = new Uri(request.RequestUri.OriginalString.Replace(request.RequestUri.Query, "") + "?" +
                                         await UrlUtils.CollectionToQueryStringAsync(queryStrings));
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
