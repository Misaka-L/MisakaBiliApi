using System.Text.Json.Serialization;

namespace MisakaBiliCore.Models.BiliApi;

public class BiliTicketResponse
{
    [JsonPropertyName("ticket")] public string Ticket { get; set; }
    [JsonPropertyName("created_at")] public long CreatedAt { get; set; }
    [JsonPropertyName("ttl")] public long Ttl { get; set; }
    [JsonPropertyName("nav")] public Nav Nav { get; set; }
}

public class Nav
{
    [JsonPropertyName("img")] public Uri Img { get; set; }
    [JsonPropertyName("sub")] public Uri Sub { get; set; }
}
