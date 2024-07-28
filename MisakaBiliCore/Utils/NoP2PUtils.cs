using System.Text.RegularExpressions;

namespace MisakaBiliCore.Utils;

public static partial class NoP2PUtils
{
    [GeneratedRegex("(mcdn.bilivideo.(cn|com)|szbdyd.com)")]
    public static partial Regex P2PRegex();

    public static string[] GetNoP2PUrls(string[] urls)
    {
        var p2pRegex = P2PRegex();
        return urls.Where(url => !p2pRegex.IsMatch(url)).ToArray();
    }

    public static string GetNoP2PUrl(string[] urls)
    {
        return GetNoP2PUrls(urls).First();
    }
}
