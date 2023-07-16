using MisakaBiliApi.Models.Bili;
using Refit;

namespace MisakaBiliApi.Services;

public interface IBiliApiServer
{
    [Get("/x/player/playurl")]
    public Task<BiliApiResponse<BiliVideoUrlResponse>> GetVideoUrlByBvid(string bvid, long cid,
        [AliasAs("qn")] int quality, [AliasAs("fnval")] int streamType,
        [AliasAs("fourk")] int allow4K = 1, [AliasAs("fnver")] int streamTypeVersion = 0, string platform = "ios",
        [AliasAs("access_key")] string accessKey = "49fac2d6e7bb84499dffc2b4e2c94171",
        [AliasAs("appkey")] string appKey = "27eb53fc9058f8c3");
    
    [Get("/x/player/playurl")]
    public Task<BiliApiResponse<BiliVideoUrlResponse>> GetVideoUrlByAvid(string avid, long cid,
        [AliasAs("qn")] int quality, [AliasAs("fnval")] int streamType,
        [AliasAs("fourk")] int allow4K = 1, [AliasAs("fnver")] int streamTypeVersion = 0, string platform = "ios",
        [AliasAs("access_key")] string accessKey = "49fac2d6e7bb84499dffc2b4e2c94171",
        [AliasAs("appkey")] string appKey = "27eb53fc9058f8c3");

    [Get("/x/web-interface/view")]
    public Task<BiliApiResponse<BiliVideoDetail>> GetVideoDetailByBvid(string bvid);
    
    [Get("/x/web-interface/view")]
    public Task<BiliApiResponse<BiliVideoDetail>> GetVideoDetailByAid(string aid);
}