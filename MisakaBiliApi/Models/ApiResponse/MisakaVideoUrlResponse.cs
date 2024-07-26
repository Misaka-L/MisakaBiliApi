using System.Text.Json.Serialization;
using MisakaBiliApi.Models.BiliApi;

namespace MisakaBiliApi.Models.ApiResponse;

/// <summary>
/// 视频流地址响应数据
/// </summary>
/// <param name="Url">视频流地址</param>
/// <param name="Format">视频格式，如 mp4720</param>
/// <param name="TimeLength">时长，单位为毫秒</param>
/// <param name="Quality">视频质量 <see href="https://github.com/SocialSisterYi/bilibili-API-collect/blob/master/docs/video/videostream_url.md#qn%E8%A7%86%E9%A2%91%E6%B8%85%E6%99%B0%E5%BA%A6%E6%A0%87%E8%AF%86"/></param>
public record MisakaVideoUrlResponse([property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("format")] string Format, [property: JsonPropertyName("timelength")]
    long TimeLength, [property: JsonPropertyName("quality")]
    BiliVideoQuality Quality);