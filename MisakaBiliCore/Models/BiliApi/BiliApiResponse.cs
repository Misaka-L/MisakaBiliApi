using System.Text.Json.Serialization;

namespace MisakaBiliCore.Models.BiliApi;

public record BiliApiResponse(
    [property: JsonPropertyName("code")] int Code,
    [property: JsonPropertyName("message")]
    string Message,
    [property: JsonPropertyName("ttl")] int Ttl);

public record BiliApiResponse<T>(
    int Code,
    string Message,
    int Ttl,
    [property: JsonPropertyName("data")] T Data) : BiliApiResponse(Code, Message, Ttl);
