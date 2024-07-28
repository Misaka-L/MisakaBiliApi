using System.Text.Json.Serialization;

namespace MisakaBiliCore.Models.BiliApi;

#region Common

public record BiliVideoUrlResponseBase
{
    [JsonPropertyName("from")] public string From { get; init; }

    [JsonPropertyName("result")] public string Result { get; init; }

    [JsonPropertyName("message")] public string Message { get; init; }

    [JsonPropertyName("quality")] public BiliVideoQuality Quality { get; init; }

    [JsonPropertyName("format")] public string Format { get; init; }

    [JsonPropertyName("timelength")] public long Timelength { get; init; }

    [JsonPropertyName("accept_format")] public string AcceptFormat { get; init; }

    [JsonPropertyName("accept_description")]
    public string[] AcceptDescription { get; init; }

    [JsonPropertyName("accept_quality")] public long[] AcceptQuality { get; init; }

    [JsonPropertyName("video_codecid")] public long VideoCodecid { get; init; }

    [JsonPropertyName("seek_param")] public string SeekParam { get; init; }

    [JsonPropertyName("seek_type")] public string SeekType { get; init; }

    [JsonPropertyName("support_formats")] public BiliVideoSupportFormat[] SupportFormats { get; init; }

    [JsonPropertyName("high_format")] public object HighFormat { get; init; }

    [JsonPropertyName("last_play_time")] public long LastPlayTime { get; init; }

    [JsonPropertyName("last_play_cid")] public long LastPlayCid { get; init; }
}

public record BiliVideoSupportFormat
{
    [JsonPropertyName("quality")] public long Quality { get; init; }

    [JsonPropertyName("format")] public string Format { get; init; }

    [JsonPropertyName("new_description")] public string NewDescription { get; init; }

    [JsonPropertyName("display_desc")] public string DisplayDesc { get; init; }

    [JsonPropertyName("superscript")] public string Superscript { get; init; }

    [JsonPropertyName("codecs")] public object Codecs { get; init; }
}

#endregion

#region Mp4

public record BiliVideoMp4UrlResponse : BiliVideoUrlResponseBase
{
    [JsonPropertyName("durl")] public BiliVideoMp4UrlItem[] Durl { get; init; } = [];
}

public record BiliVideoMp4UrlItem
{
    [JsonPropertyName("order")] public long Order { get; init; }

    [JsonPropertyName("length")] public long Length { get; init; }

    [JsonPropertyName("size")] public long Size { get; init; }

    [JsonPropertyName("ahead")] public string Ahead { get; init; }

    [JsonPropertyName("vhead")] public string Vhead { get; init; }

    [JsonPropertyName("url")] public string Url { get; init; }

    [JsonPropertyName("backup_url")] public string[]? BackupUrl { get; init; }
}

#endregion

#region Dash

public record BiliVideoDashUrlResponse : BiliVideoUrlResponseBase
{
    [JsonPropertyName("dash")] public BiliVideoDashInfo Dash { get; init; }
}

public record BiliVideoDashInfo
{
    [JsonPropertyName("duration")] public long Duration { get; set; }

    [JsonPropertyName("minBufferTime")] public double MinBufferTime { get; set; }

    [JsonPropertyName("min_buffer_time")] public double TemperaturesMinBufferTime { get; set; }

    [JsonPropertyName("video")] public BiliVideoDashUrlItem[] Video { get; set; }

    [JsonPropertyName("audio")] public BiliVideoDashUrlItem[] Audio { get; set; }

    [JsonPropertyName("dolby")] public BiliVideoDashDolbyInfo BiliVideoDashDolbyInfo { get; set; }

    [JsonPropertyName("flac")] public object Flac { get; set; }
}

public record BiliVideoDashUrlItem
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("base_url")] public Uri BaseUrl { get; set; }

    [JsonPropertyName("backup_url")] public Uri[]? BackupUrls { get; set; }

    [JsonPropertyName("bandwidth")] public long Bandwidth { get; set; }

    [JsonPropertyName("mime_type")] public string MimeType { get; set; }

    [JsonPropertyName("codecs")] public string Codecs { get; set; }

    [JsonPropertyName("width")] public long Width { get; set; }

    [JsonPropertyName("height")] public long Height { get; set; }

    [JsonPropertyName("frame_rate")] public string FrameRate { get; set; }

    [JsonPropertyName("sar")] public string Sar { get; set; }

    [JsonPropertyName("startWithSap")] public long StartWithSap { get; set; }

    [JsonPropertyName("start_with_sap")] public long AudioStartWithSap { get; set; }

    [JsonPropertyName("segment_base")] public BiliVideoDashItemSegmentBase egmentBase { get; set; }

    [JsonPropertyName("codecid")] public long Codecid { get; set; }
}

public record BiliVideoDashItemSegmentBase
{
    [JsonPropertyName("initialization")] public string Initialization { get; set; }
    [JsonPropertyName("index_range")] public string IndexRange { get; set; }
}

public record BiliVideoDashDolbyInfo
{
    [JsonPropertyName("type")] public long Type { get; set; }
    [JsonPropertyName("audio")] public object Audio { get; set; }
}

#endregion
