using System.Text.Json.Serialization;

namespace MisakaBiliApi.Models.Bili;

public record BiliVideoDetail
{
    [JsonPropertyName("bvid")] public string Bvid { get; init; }

    [JsonPropertyName("aid")] public long Aid { get; init; }

    [JsonPropertyName("videos")] public long Videos { get; init; }

    [JsonPropertyName("tid")] public long Tid { get; init; }

    [JsonPropertyName("tname")] public string Tname { get; init; }

    [JsonPropertyName("copyright")] public BiliCopyrightType Copyright { get; init; }

    [JsonPropertyName("pic")] public string CoverUrl { get; init; }

    [JsonPropertyName("title")] public string Title { get; init; }

    [JsonPropertyName("duration")] public long Duration { get; init; }

    [JsonPropertyName("owner")] public BiliVideoOwner Owner { get; init; }

    [JsonPropertyName("stat")] public BiliVideoStatue Statue { get; init; }

    [JsonPropertyName("cid")] public long Cid { get; init; }

    [JsonPropertyName("dimension")] public Dimension Dimension { get; init; }

    [JsonPropertyName("pages")] public BiliVideoPage[] Pages { get; init; }

    [JsonPropertyName("staff")] public BiliVideoStaff[] Staff { get; init; }
}

public enum BiliCopyrightType
{
    Original = 1,
    Reprint = 2
}

public record Dimension
{
    [JsonPropertyName("width")] public long Width { get; init; }

    [JsonPropertyName("height")] public long Height { get; init; }

    [JsonPropertyName("rotate")] public long Rotate { get; init; }
}
public record BiliVideoOwner
{
    [JsonPropertyName("mid")] public long Mid { get; init; }

    [JsonPropertyName("name")] public string Name { get; init; }

    [JsonPropertyName("face")] public Uri Face { get; init; }
}

public record BiliVideoPage
{
    [JsonPropertyName("cid")] public long Cid { get; init; }

    [JsonPropertyName("page")] public long PagePage { get; init; }

    [JsonPropertyName("from")] public string From { get; init; }

    [JsonPropertyName("part")] public string Part { get; init; }

    [JsonPropertyName("duration")] public long Duration { get; init; }

    [JsonPropertyName("vid")] public string Vid { get; init; }

    [JsonPropertyName("weblink")] public string Weblink { get; init; }

    [JsonPropertyName("dimension")] public Dimension Dimension { get; init; }

    [JsonPropertyName("first_frame")] public Uri FirstFrame { get; init; }
}

public record BiliVideoStaff
{
    [JsonPropertyName("mid")] public long Mid { get; init; }

    [JsonPropertyName("title")] public string Title { get; init; }

    [JsonPropertyName("name")] public string Name { get; init; }

    [JsonPropertyName("face")] public Uri Face { get; init; }

    [JsonPropertyName("follower")] public long Follower { get; init; }
}

public record BiliVideoStatue
{
    [JsonPropertyName("aid")] public long Aid { get; init; }

    [JsonPropertyName("view")] public long View { get; init; }

    [JsonPropertyName("danmaku")] public long Danmaku { get; init; }

    [JsonPropertyName("reply")] public long Reply { get; init; }

    [JsonPropertyName("favorite")] public long Favorite { get; init; }

    [JsonPropertyName("coin")] public long Coin { get; init; }

    [JsonPropertyName("share")] public long Share { get; init; }

    [JsonPropertyName("now_rank")] public long NowRank { get; init; }

    [JsonPropertyName("his_rank")] public long HisRank { get; init; }

    [JsonPropertyName("like")] public long Like { get; init; }

    [JsonPropertyName("dislike")] public long Dislike { get; init; }

    [JsonPropertyName("evaluation")] public string Evaluation { get; init; }

    [JsonPropertyName("argue_msg")] public string ArgueMsg { get; init; }

    [JsonPropertyName("vt")] public long Vt { get; init; }
}