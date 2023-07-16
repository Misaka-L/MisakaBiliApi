using System.Text.Json.Serialization;

namespace MisakaBiliApi.Models.ApiResponse;

public record MisakaApiResponse<T>([property: JsonPropertyName("data")] T Data,
    [property: JsonPropertyName("message")]
    string Message = "");