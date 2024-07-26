using MisakaBiliCore.Models.BiliApi;
using Refit;

namespace MisakaBiliCore.Services.BiliApi;

public interface IBiliApiServices
{
    [Get("/x/player/wbi/playurl")]
    public Task<BiliApiResponse<BiliVideoUrlResponse>> GetVideoUrlByBvid(string bvid, long cid,
        [AliasAs("qn")] int quality, [AliasAs("fnval")] int streamType,
        [AliasAs("fourk")] int allow4K = 1, [AliasAs("fnver")] int streamTypeVersion = 0, string platform = "html5",
        [AliasAs("high_quality")] int highQuality = 1);

    [Get("/x/player/wbi/playurl")]
    public Task<BiliApiResponse<BiliVideoUrlResponse>> GetVideoUrlByAvid(string avid, long cid,
        [AliasAs("qn")] int quality, [AliasAs("fnval")] int streamType,
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
}
