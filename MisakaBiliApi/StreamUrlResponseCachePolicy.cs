using Microsoft.AspNetCore.OutputCaching;
using Microsoft.Extensions.Primitives;

namespace MisakaBiliApi;

public class StreamUrlResponseCachePolicy(string[] queryStringKeys) : IOutputCachePolicy
{
    ValueTask IOutputCachePolicy.CacheRequestAsync(
        OutputCacheContext context,
        CancellationToken cancellation)
    {
        var attemptOutputCaching = AttemptOutputCaching(context);
        context.EnableOutputCaching = true;
        context.AllowCacheLookup = attemptOutputCaching;
        context.AllowCacheStorage = attemptOutputCaching;
        context.AllowLocking = true;

        // Vary by any query by default
        context.CacheVaryByRules.QueryKeys = queryStringKeys;

        return ValueTask.CompletedTask;
    }

    ValueTask IOutputCachePolicy.ServeFromCacheAsync
        (OutputCacheContext context, CancellationToken cancellation)
    {
        return ValueTask.CompletedTask;
    }

    ValueTask IOutputCachePolicy.ServeResponseAsync
        (OutputCacheContext context, CancellationToken cancellation)
    {
        var response = context.HttpContext.Response;

        // Verify existence of cookie headers
        if (!StringValues.IsNullOrEmpty(response.Headers.SetCookie))
        {
            context.AllowCacheStorage = false;
            return ValueTask.CompletedTask;
        }

        // Check response code
        if (response.StatusCode != StatusCodes.Status200OK &&
            response.StatusCode != StatusCodes.Status302Found)
        {
            context.AllowCacheStorage = false;
            return ValueTask.CompletedTask;
        }

        return ValueTask.CompletedTask;
    }

    private static bool AttemptOutputCaching(OutputCacheContext context)
    {
        // Check if the current request fulfills the requirements
        // to be cached
        var request = context.HttpContext.Request;

        // Verify the method
        if (!HttpMethods.IsGet(request.Method) &&
            !HttpMethods.IsHead(request.Method))
        {
            return false;
        }

        // Verify existence of authorization headers
        if (!StringValues.IsNullOrEmpty(request.Headers.Authorization) ||
            request.HttpContext.User?.Identity?.IsAuthenticated == true)
        {
            return false;
        }

        return true;
    }
}
