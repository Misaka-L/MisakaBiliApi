using System.Text.Json.Serialization;
using MisakaBiliApi.Models.Bili;

namespace MisakaBiliApi.Models.ApiResponse;

public record MisakaVideoUrlResponse([property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("format")] string Format, [property: JsonPropertyName("timelength")]
    long TimeLength, [property: JsonPropertyName("quality")]
    BiliVideoQuality Quality);