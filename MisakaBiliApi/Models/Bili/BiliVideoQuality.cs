namespace MisakaBiliApi.Models.Bili;

public enum BiliVideoQuality
{
    R240P = 6,
    R380P = 16,
    R480P = 32,
    R720P = 64,
    R720P60 = 74,
    R1080P = 80,
    R1080PHighRate = 112,
    R1080P60Fps = 116,
    R4K = 120
}

public enum BiliVideoStreamType
{
    Mp4 = 1,
    R4K = 128
}