using System.Text.Json.Serialization;

namespace MisakaBiliApi.Models.Bili;

public record BiliApiResponse<T>([property: JsonPropertyName("code")] int Code, [property: JsonPropertyName("message")]
    string Message, [property: JsonPropertyName("ttl")] int Ttl, [property: JsonPropertyName("data")] T Data);