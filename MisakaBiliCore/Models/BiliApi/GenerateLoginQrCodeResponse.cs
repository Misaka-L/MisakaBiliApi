using System.Text.Json.Serialization;

namespace MisakaBiliCore.Models.BiliApi;

public record GenerateLoginQrCodeResponse(
    [property: JsonPropertyName("url")] string Url,
    [property: JsonPropertyName("qrcode_key")] string QrCodeKey
    );
