using Yarp.ReverseProxy.Forwarder;

namespace MisakaBiliApi.Forwarder;

public class BiliVideoTransformer : HttpTransformer
{
    public override async ValueTask TransformRequestAsync(HttpContext httpContext,
        HttpRequestMessage proxyRequest,
        string destinationPrefix,
        CancellationToken cancellationToken)
    {
        if (!httpContext.Request.Path.HasValue) return;

        await base.TransformRequestAsync(httpContext, proxyRequest, destinationPrefix, cancellationToken);

        var requestUri = new Uri("https://" + httpContext.Request.Path.Value.Replace("/forward/bilibili/", "") +
                                 httpContext.Request.QueryString);
        proxyRequest.RequestUri = requestUri;
        proxyRequest.Headers.Host = requestUri.Host;
        proxyRequest.Headers.Referrer = new Uri("https://www.bilibili.com");
        proxyRequest.Headers.UserAgent.ParseAdd(
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/114.0.0.0 Safari/537.36 Edg/114.0.1823.79");
        // httpContext.Request.Path = new PathString(httpContext.Request.Path.Value?.Replace("/forward/bilibili", ""));
    }
}