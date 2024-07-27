using System.Text.Json.Serialization;

namespace MisakaBiliCore.Models.BiliApi;

public class BiliLiveUrlResponse
{
    [JsonPropertyName("current_quality")] public long CurrentQuality { get; set; }

    [JsonPropertyName("accept_quality")] public string[] AcceptQuality { get; set; }

    [JsonPropertyName("current_qn")] public long CurrentQn { get; set; }

    [JsonPropertyName("quality_description")]
    public QualityDescription[] QualityDescription { get; set; }

    [JsonPropertyName("durl")] public Durl[] Durl { get; set; }
}

public class Durl
{
    [JsonPropertyName("url")] public Uri Url { get; set; }

    [JsonPropertyName("length")] public long Length { get; set; }

    [JsonPropertyName("order")] public long Order { get; set; }

    [JsonPropertyName("stream_type")] public long StreamType { get; set; }

    [JsonPropertyName("p2p_type")] public long P2PType { get; set; }
}

public class QualityDescription
{
    [JsonPropertyName("qn")] public long Qn { get; set; }

    [JsonPropertyName("desc")] public string Desc { get; set; }
}
