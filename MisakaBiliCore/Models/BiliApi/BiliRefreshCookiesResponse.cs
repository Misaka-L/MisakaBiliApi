using System.Text.Json.Serialization;

namespace MisakaBiliCore.Models.BiliApi;

public class BiliRefreshCookiesResponse
{
    [JsonPropertyName("status")] public long Status { get; set; }

    [JsonPropertyName("message")] public string Message { get; set; }

    [JsonPropertyName("refresh_token")] public string RefreshToken { get; set; }
}
