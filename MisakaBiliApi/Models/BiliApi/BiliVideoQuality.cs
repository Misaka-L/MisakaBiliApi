namespace MisakaBiliApi.Models.BiliApi;

/// <summary>
/// 哔哩哔哩视频质量参数（qn）
/// <see href="https://github.com/SocialSisterYi/bilibili-API-collect/blob/master/docs/video/videostream_url.md#qn%E8%A7%86%E9%A2%91%E6%B8%85%E6%99%B0%E5%BA%A6%E6%A0%87%E8%AF%86"/>
/// </summary>
public enum BiliVideoQuality
{
    /// <summary>
    /// 240P
    /// </summary>
    R240P = 6,
    /// <summary>
    /// 380P
    /// </summary>
    R380P = 16,
    /// <summary>
    /// 480P
    /// </summary>
    R480P = 32,
    /// <summary>
    /// 720P
    /// </summary>
    R720P = 64,
    /// <summary>
    /// 720P 60FPS
    /// </summary>
    R720P60 = 74,
    /// <summary>
    /// 1080P
    /// </summary>
    R1080P = 80,
    /// <summary>
    /// 1080P 高码率
    /// </summary>
    R1080PHighRate = 112,
    /// <summary>
    /// 1080P 60FPS
    /// </summary>
    R1080P60Fps = 116,
    /// <summary>
    /// 4K
    /// </summary>
    R4K = 120
}

/// <summary>
/// 哔哩哔哩视频流参数 <see href="https://github.com/SocialSisterYi/bilibili-API-collect/blob/master/docs/video/videostream_url.md"/>
/// </summary>
public enum BiliVideoStreamType
{
    /// <summary>
    /// MP4
    /// </summary>
    Mp4 = 1,
    /// <summary>
    /// 4K
    /// </summary>
    R4K = 128
}
