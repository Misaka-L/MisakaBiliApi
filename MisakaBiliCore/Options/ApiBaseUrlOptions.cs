namespace MisakaBiliCore.Options;

public class ApiBaseUrlOptions
{
    public string BiliApiBaseUrl { get; set; } = "https://api.bilibili.com";
    public string BiliPassportBaseUrl { get; set; } = "https://passport.bilibili.com";
    public string BiliWebBaseUrl { get; set; } = "https://www.bilibili.com";
    public string BiliLiveApiBaseUrl { get; set; } = "https://api.live.bilibili.com";

    public string BiliApiHost { get; set; } = "api.bilibili.com";
    public string BiliPassportHost { get; set; } = "passport.bilibili.com";
    public string BiliWebHost { get; set; } = "www.bilibili.com";
    public string BiliLiveApiHost { get; set; } = "api.live.bilibili.com";
}
