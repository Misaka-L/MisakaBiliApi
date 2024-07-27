using MisakaBiliCore.Models.BiliApi;
using Refit;

namespace MisakaBiliCore.Services.BiliApi;

public interface IBiliLiveApiService
{
    [Get("/room/v1/Room/playUrl")]
    public Task<BiliApiResponse<BiliLiveUrlResponse>> GetLiveUrlByRoomId(
        [AliasAs("cid")] string cid,
        [AliasAs("platform")] string platform = "web",
        [AliasAs("quality")] int quality = (int)BiliLiveQuality.Original);
}
