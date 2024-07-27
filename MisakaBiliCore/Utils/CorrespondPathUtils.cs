using System.Security.Cryptography;
using System.Text;

namespace MisakaBiliCore.Utils;

public static class CorrespondPathUtils
{
    public static string GetCorrespondPath(DateTimeOffset? dateTimeOffset = null)
    {
        var pem =
            "-----BEGIN PUBLIC KEY-----" +
            "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDLgd2OAkcGVtoE3ThUREbio0Eg" +
            "Uc/prcajMKXvkCKFCWhJYJcLkcM2DKKcSeFpD/j6Boy538YXnR6VhcuUJOhH2x71" +
            "nzPjfdTcqMz7djHum0qSZA0AyCBDABUqCrfNgCiJ00Ra7GmRj+YCK1NJEuewlb40" +
            "JNrRuoEUXpabUzGB8QIDAQAB" +
            "-----END PUBLIC KEY-----";

        var dateTime = dateTimeOffset ?? DateTimeOffset.Now;
        var refreshMessage = $"refresh_{dateTime.ToUnixTimeMilliseconds()}";
        var refreshMessageBytes = Encoding.UTF8.GetBytes(refreshMessage);

        using var rsa = RSA.Create();
        rsa.ImportFromPem(pem);

        var encryptedData = rsa.Encrypt(refreshMessageBytes, RSAEncryptionPadding.OaepSHA256);

        return Convert.ToHexString(encryptedData).ToLower();
    }
}
