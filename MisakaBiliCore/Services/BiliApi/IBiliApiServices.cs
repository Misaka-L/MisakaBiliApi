using MisakaBiliCore.Models.BiliApi;
using Refit;

namespace MisakaBiliCore.Services.BiliApi;

public interface IBiliApiServices
{
    [Get("/x/player/wbi/playurl")]
    public Task<BiliApiResponse<BiliVideoMp4UrlResponse>> GetVideoMp4UrlByBvid(string bvid, long cid,
        [AliasAs("qn")] int quality, [AliasAs("fnval")] int streamType = (int)BiliVideoStreamType.Mp4,
        [AliasAs("fourk")] int allow4K = 1, [AliasAs("fnver")] int streamTypeVersion = 0, string platform = "html5",
        [AliasAs("high_quality")] int highQuality = 1);

    [Get("/x/player/wbi/playurl")]
    public Task<BiliApiResponse<BiliVideoMp4UrlResponse>> GetVideoMp4UrlByAvid(string avid, long cid,
        [AliasAs("qn")] int quality, [AliasAs("fnval")] int streamType = (int)BiliVideoStreamType.Mp4,
        [AliasAs("fourk")] int allow4K = 1, [AliasAs("fnver")] int streamTypeVersion = 0, string platform = "html5",
        [AliasAs("high_quality")] int highQuality = 1,
        [AliasAs("access_key")] string accessKey = "49fac2d6e7bb84499dffc2b4e2c94171",
        [AliasAs("appkey")] string appKey = "27eb53fc9058f8c3");

    [Get("/x/player/wbi/playurl")]
    public Task<BiliApiResponse<BiliVideoDashUrlResponse>> GetVideoDashUrlByBvid(string bvid, long cid,
        [AliasAs("qn")] int quality, [AliasAs("fnval")] int streamType = (int)BiliVideoStreamType.Dash,
        [AliasAs("fourk")] int allow4K = 1, [AliasAs("fnver")] int streamTypeVersion = 0, string platform = "html5",
        [AliasAs("high_quality")] int highQuality = 1);

    [Get("/x/player/wbi/playurl")]
    public Task<BiliApiResponse<BiliVideoDashUrlResponse>> GetVideoDashUrlByAvid(string avid, long cid,
        [AliasAs("qn")] int quality, [AliasAs("fnval")] int streamType = (int)BiliVideoStreamType.Dash,
        [AliasAs("fourk")] int allow4K = 1, [AliasAs("fnver")] int streamTypeVersion = 0, string platform = "html5",
        [AliasAs("high_quality")] int highQuality = 1,
        [AliasAs("access_key")] string accessKey = "49fac2d6e7bb84499dffc2b4e2c94171",
        [AliasAs("appkey")] string appKey = "27eb53fc9058f8c3");

    [Get("/x/web-interface/view")]
    public Task<BiliApiResponse<BiliVideoDetail>> GetVideoDetailByBvid(string bvid);

    [Get("/x/web-interface/view")]
    public Task<BiliApiResponse<BiliVideoDetail>> GetVideoDetailByAid(string aid);

    [Get("/x/space/myinfo")]
    public Task<BiliApiResponse<BiliUser>> GetLoginUserInfo();

    [Get("/x/web-interface/nav")]
    public Task<BiliApiResponse<BiliNavMenuResponse>> GetNavMenu();
}
