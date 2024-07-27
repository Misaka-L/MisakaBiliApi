using System.Text.Json.Serialization;

namespace MisakaBiliCore.Models.BiliApi;

public class BiliNavMenuResponse
{
    [JsonPropertyName("wbi_img")] public BiliNavMenuWbiImage WbiImage { get; set; }
}

public class BiliNavMenuWbiImage
{
    [JsonPropertyName("img_url")] public string ImgUrl { get; set; }
    [JsonPropertyName("sub_url")] public string SubUrl { get; set; }
}

