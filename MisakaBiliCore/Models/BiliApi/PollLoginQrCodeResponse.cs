using System.Text.Json.Serialization;

namespace MisakaBiliCore.Models.BiliApi;

public record PollLoginQrCodeData(
    [property: JsonPropertyName("refresh_token")]
    string? RefreshToken,
    [property: JsonPropertyName("code")] QrCodeLoginStatus Code,
    [property: JsonPropertyName("timestamp")]
    long Timestamp,
    [property: JsonPropertyName("message")]
    string Message,
    [property: JsonPropertyName("url")] string? Url
);
