using System.Text.Json.Serialization;

namespace MisakaBiliApi.Models.Bili;

    public record BiliVideoUrlResponse
    {
        [JsonPropertyName("from")]
        public string From  { get; init; }

        [JsonPropertyName("result")]
        public string Result  { get; init; }

        [JsonPropertyName("message")]
        public string Message  { get; init; }

        [JsonPropertyName("quality")]
        public BiliVideoQuality Quality  { get; init; }

        [JsonPropertyName("format")]
        public string Format  { get; init; }

        [JsonPropertyName("timelength")]
        public long Timelength  { get; init; }

        [JsonPropertyName("accept_format")]
        public string AcceptFormat  { get; init; }

        [JsonPropertyName("accept_description")]
        public string[] AcceptDescription  { get; init; }

        [JsonPropertyName("accept_quality")]
        public long[] AcceptQuality  { get; init; }

        [JsonPropertyName("video_codecid")]
        public long VideoCodecid  { get; init; }

        [JsonPropertyName("seek_param")]
        public string SeekParam  { get; init; }

        [JsonPropertyName("seek_type")]
        public string SeekType  { get; init; }

        [JsonPropertyName("durl")]
        public BiliVideoUrlItem[] Durl  { get; init; }

        [JsonPropertyName("support_formats")]
        public BiliVideoSupportFormat[] SupportFormats  { get; init; }

        [JsonPropertyName("high_format")]
        public object HighFormat  { get; init; }

        [JsonPropertyName("last_play_time")]
        public long LastPlayTime  { get; init; }

        [JsonPropertyName("last_play_cid")]
        public long LastPlayCid  { get; init; }
    }

    public record BiliVideoUrlItem
    {
        [JsonPropertyName("order")]
        public long Order  { get; init; }

        [JsonPropertyName("length")]
        public long Length  { get; init; }

        [JsonPropertyName("size")]
        public long Size  { get; init; }

        [JsonPropertyName("ahead")]
        public string Ahead  { get; init; }

        [JsonPropertyName("vhead")]
        public string Vhead  { get; init; }

        [JsonPropertyName("url")]
        public string Url  { get; init; }

        [JsonPropertyName("backup_url")]
        public string[]? BackupUrl  { get; init; }
    }

    public record BiliVideoSupportFormat
    {
        [JsonPropertyName("quality")]
        public long Quality  { get; init; }

        [JsonPropertyName("format")]
        public string Format  { get; init; }

        [JsonPropertyName("new_description")]
        public string NewDescription  { get; init; }

        [JsonPropertyName("display_desc")]
        public string DisplayDesc  { get; init; }

        [JsonPropertyName("superscript")]
        public string Superscript  { get; init; }

        [JsonPropertyName("codecs")]
        public object Codecs  { get; init; }
    }
