using System.Text.Json.Serialization;

namespace MisakaBiliApi.Models.ApiResponse;

/// <summary>
/// API 响应数据
/// </summary>
/// <param name="Message">响应消息</param>
/// <param name="Code">响应代码（通常为 HTTP 折腾码）</param>
public record MisakaApiResponse([property: JsonPropertyName("message")]
    string Message = "", [property: JsonPropertyName("code")] int Code = StatusCodes.Status200OK);

/// <summary>
/// API 响应数据
/// </summary>
/// <param name="Message">响应消息</param>
/// <param name="Data">响应数据</param>
/// <param name="Code">响应代码（通常为 HTTP 状态码）</param>
/// <typeparam name="T">响应数据类型</typeparam>
public record MisakaApiResponse<T>(string Message,
    [property: JsonPropertyName("data")] T Data, int Code = StatusCodes.Status200OK) : MisakaApiResponse;