using System.Text.RegularExpressions;

namespace MisakaBiliCore.Utils;

public static partial class NoP2PUtils
{
    [GeneratedRegex("(mcdn.bilivideo.(cn|com)|szbdyd.com)")]
    private static partial Regex P2PRegex();

    public static string GetNoP2PUrl(string[] urls)
    {
        var p2pRegex = P2PRegex();
        return urls.First(url => !p2pRegex.IsMatch(url));
    }
}
