using System.Security.Cryptography;
using System.Text;

namespace MisakaBiliCore.Utils;

public static class WbiUtils
{
    public static readonly int[] MixinKeyEncTab =
    {
        46, 47, 18, 2, 53, 8, 23, 32, 15, 50, 10, 31, 58, 3, 45, 35, 27, 43, 5, 49, 33, 9, 42, 19, 29, 28, 14, 39,
        12, 38, 41, 13, 37, 48, 7, 16, 24, 55, 40, 61, 26, 17, 0, 1, 60, 51, 30, 4, 22, 25, 54, 21, 56, 59, 6, 63,
        57, 62, 11, 36, 20, 34, 44, 52
    };

    public static string GetMixinKey(string original)
    {
        return MixinKeyEncTab.Aggregate("", (s, i) => s + original[i])[..32];
    }

    public static async Task<string> GetWRidAsync(Dictionary<string, string> rawQueryString, string mixinKey, DateTimeOffset dateTime)
    {
        var rawQueryStringClone = rawQueryString.ToDictionary();
        rawQueryStringClone.Add("wts", dateTime.ToUnixTimeSeconds().ToString());

        var queryString = await UrlUtils.CollectionToQueryStringAsync(
            rawQueryStringClone
                .OrderBy(item => item.Key)
                .ToDictionary()
        ) + mixinKey;

        return Convert.ToHexString(MD5.HashData(Encoding.UTF8.GetBytes(queryString))).ToLower();
    }
}
