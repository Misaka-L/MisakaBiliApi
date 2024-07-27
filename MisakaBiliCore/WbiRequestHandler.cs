using System.Net.Http.Json;
using MisakaBiliCore.Models.BiliApi;
using MisakaBiliCore.Services;
using MisakaBiliCore.Services.BiliApi;
using MisakaBiliCore.Utils;

namespace MisakaBiliCore;

public class WbiRequestHandler(
    BiliApiSecretStorageService biliApiSecretStorageService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (request.RequestUri.LocalPath.Contains("/wbi/"))
        {
            var dateTime = TimeProvider.System.GetUtcNow();
            var queryStrings = UrlUtils.GetQueryStringCollection(request.RequestUri.Query);

            var mixinKey =
                WbiUtils.GetMixinKey(biliApiSecretStorageService.WbiImgKey + biliApiSecretStorageService.WbiSubKey);
            var wrid = await WbiUtils.GetWRidAsync(queryStrings, mixinKey, dateTime);

            queryStrings.Add("w_rid", wrid);
            queryStrings.Add("wts", dateTime.ToUnixTimeSeconds().ToString());

            request.RequestUri = new Uri(request.RequestUri.OriginalString.Replace(request.RequestUri.Query, "") + "?" +
                                         await UrlUtils.CollectionToQueryStringAsync(queryStrings));
        }

        return await base.SendAsync(request, cancellationToken);
    }
}
