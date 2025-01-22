namespace MisakaBiliCore.Utils;

public static class BvAvUtils
{
    private const long XorCode = 23442827791579L;
    private const long MaskCode = 2251799813685247L;
    private const long MaxAid = 1L << 51;
    private const long Base = 58L;

    private const string Data = "FcwAPNKTMug3GV5Lj7EJnHpWsx4tb8haYeviqBz6rkCy12mUSDQX9RdoZf";

    // https://socialsisteryi.github.io/bilibili-API-collect/docs/misc/bvid_desc.html

    public static string AvToBv(long aid)
    {
        var bytes = new[] { 'B', 'V', '1', '0', '0', '0', '0', '0', '0', '0', '0', '0' };
        var bvIndex = bytes.Length - 1;
        var tmp = (MaxAid | aid) ^ XorCode;
        while (tmp > 0)
        {
            bytes[bvIndex] = Data[(int)(tmp % Base)];
            tmp /= Base;
            bvIndex -= 1;
        }
        (bytes[3], bytes[9]) = (bytes[9], bytes[3]);
        (bytes[4], bytes[7]) = (bytes[7], bytes[4]);
        return new string(bytes);
    }

    public static long BvToAv(string bvid)
    {
        var bvidArr = bvid.ToCharArray();
        (bvidArr[3], bvidArr[9]) = (bvidArr[9], bvidArr[3]);
        (bvidArr[4], bvidArr[7]) = (bvidArr[7], bvidArr[4]);
        bvidArr = bvidArr.Skip(3).ToArray();
        var tmp = bvidArr.Aggregate(0L, (pre, bvidChar) => pre * Base + Data.IndexOf(bvidChar));
        return (tmp & MaskCode) ^ XorCode;
    }
}
