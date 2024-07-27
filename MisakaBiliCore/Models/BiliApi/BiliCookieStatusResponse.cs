using System.Text.Json.Serialization;

namespace MisakaBiliCore.Models.BiliApi;

public record BiliCookieStatusResponse(
    [property: JsonPropertyName("refresh")] bool NeedRefresh,
    [property: JsonPropertyName("timestamp")] long Timestamp
    );
