using System.Text.Json.Serialization;

namespace MisakaBiliCore.Models.BiliApi;

public partial class BiliUser
{
    [JsonPropertyName("mid")] public long Mid { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("sex")] public string Sex { get; set; }

    [JsonPropertyName("face")] public Uri Face { get; set; }

    [JsonPropertyName("sign")] public string Sign { get; set; }

    [JsonPropertyName("rank")] public long Rank { get; set; }

    [JsonPropertyName("level")] public long Level { get; set; }

    [JsonPropertyName("jointime")] public long Jointime { get; set; }

    [JsonPropertyName("moral")] public long Moral { get; set; }

    [JsonPropertyName("silence")] public long Silence { get; set; }

    [JsonPropertyName("email_status")] public long EmailStatus { get; set; }

    [JsonPropertyName("tel_status")] public long TelStatus { get; set; }

    [JsonPropertyName("identification")] public long Identification { get; set; }

    [JsonPropertyName("vip")] public Vip Vip { get; set; }

    [JsonPropertyName("pendant")] public Pendant Pendant { get; set; }

    [JsonPropertyName("nameplate")] public Nameplate Nameplate { get; set; }

    [JsonPropertyName("official")] public Official Official { get; set; }

    [JsonPropertyName("birthday")] public long Birthday { get; set; }

    [JsonPropertyName("is_tourist")] public long IsTourist { get; set; }

    [JsonPropertyName("is_fake_account")] public long IsFakeAccount { get; set; }

    [JsonPropertyName("pin_prompting")] public long PinPrompting { get; set; }

    [JsonPropertyName("is_deleted")] public long IsDeleted { get; set; }

    [JsonPropertyName("in_reg_audit")] public long InRegAudit { get; set; }

    [JsonPropertyName("is_rip_user")] public bool IsRipUser { get; set; }

    [JsonPropertyName("profession")] public Profession Profession { get; set; }

    [JsonPropertyName("level_exp")] public LevelExp LevelExp { get; set; }

    [JsonPropertyName("coins")] public float Coins { get; set; }

    [JsonPropertyName("following")] public long Following { get; set; }

    [JsonPropertyName("follower")] public long Follower { get; set; }
}

public partial class LevelExp
{
    [JsonPropertyName("current_level")] public long CurrentLevel { get; set; }

    [JsonPropertyName("current_min")] public long CurrentMin { get; set; }

    [JsonPropertyName("current_exp")] public long CurrentExp { get; set; }

    [JsonPropertyName("next_exp")] public long NextExp { get; set; }
}

public partial class Nameplate
{
    [JsonPropertyName("nid")] public long Nid { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("image")] public Uri Image { get; set; }

    [JsonPropertyName("image_small")] public Uri ImageSmall { get; set; }

    [JsonPropertyName("level")] public string Level { get; set; }

    [JsonPropertyName("condition")] public string Condition { get; set; }
}

public partial class Official
{
    [JsonPropertyName("role")] public long Role { get; set; }

    [JsonPropertyName("title")] public string Title { get; set; }

    [JsonPropertyName("desc")] public string Desc { get; set; }

    [JsonPropertyName("type")] public long Type { get; set; }
}

public partial class Pendant
{
    [JsonPropertyName("pid")] public long Pid { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("image")] public Uri Image { get; set; }

    [JsonPropertyName("expire")] public long Expire { get; set; }

    [JsonPropertyName("image_enhance")] public Uri ImageEnhance { get; set; }

    [JsonPropertyName("image_enhance_frame")]
    public Uri ImageEnhanceFrame { get; set; }
}

public partial class Profession
{
    [JsonPropertyName("id")] public long Id { get; set; }

    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("show_name")] public string ShowName { get; set; }
}

public partial class Vip
{
    [JsonPropertyName("type")] public long Type { get; set; }

    [JsonPropertyName("status")] public long Status { get; set; }

    [JsonPropertyName("due_date")] public long DueDate { get; set; }

    [JsonPropertyName("vip_pay_type")] public long VipPayType { get; set; }

    [JsonPropertyName("theme_type")] public long ThemeType { get; set; }

    [JsonPropertyName("label")] public Label Label { get; set; }

    [JsonPropertyName("avatar_subscript")] public long AvatarSubscript { get; set; }

    [JsonPropertyName("nickname_color")] public string NicknameColor { get; set; }

    [JsonPropertyName("role")] public long Role { get; set; }

    [JsonPropertyName("avatar_subscript_url")]
    public Uri AvatarSubscriptUrl { get; set; }
}

public partial class Label
{
    [JsonPropertyName("path")] public string Path { get; set; }

    [JsonPropertyName("text")] public string Text { get; set; }

    [JsonPropertyName("label_theme")] public string LabelTheme { get; set; }

    [JsonPropertyName("text_color")] public string TextColor { get; set; }

    [JsonPropertyName("bg_style")] public long BgStyle { get; set; }

    [JsonPropertyName("bg_color")] public string BgColor { get; set; }

    [JsonPropertyName("border_color")] public string BorderColor { get; set; }
}
