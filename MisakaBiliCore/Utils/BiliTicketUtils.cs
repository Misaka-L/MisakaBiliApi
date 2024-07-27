using System.Security.Cryptography;
using System.Text;

namespace MisakaBiliCore.Utils;

public static class BiliTicketUtils
{
    public const string Key = "XgwSnGZ1p";

    public static string GetBiliTicketHexSign(DateTimeOffset? dateTime = null)
    {
        var timeStamp = (dateTime ?? TimeProvider.System.GetUtcNow()).ToUnixTimeSeconds();

        var hmacSha256 = new HMACSHA256(Encoding.UTF8.GetBytes(Key));
        var message = $"ts{timeStamp}";

        var messageHashBytes = hmacSha256.ComputeHash(Encoding.UTF8.GetBytes(message));
        var messageHash = Convert.ToHexString(messageHashBytes);

        return messageHash;
    }
}
