using System.Web;

namespace MisakaBiliCore.Utils;

public static class UrlUtils
{
    public static async Task<string> CollectionToQueryStringAsync(Dictionary<string, string> collection)
    {
        return await new FormUrlEncodedContent(collection).ReadAsStringAsync();
    }

    public static Dictionary<string, string> GetQueryStringCollection(string queryString)
    {
        var requestQueryString = HttpUtility.ParseQueryString(queryString);
        return requestQueryString.AllKeys
            .ToDictionary(
                requestQueryKey => requestQueryKey,
                requestQueryKey => requestQueryString[requestQueryKey]
            );
    }
}
