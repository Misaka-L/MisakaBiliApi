using Microsoft.AspNetCore.Mvc;
using MisakaBiliApi.Models.ApiResponse;
using MisakaBiliApi.Models.Bili;
using MisakaBiliApi.Services;

namespace MisakaBiliApi.Controllers;

[Route("/api/bilibili/viedo/")]
public class BiliVideoController : Controller
{
    private IBiliApiServer _biliApiServer;

    public BiliVideoController(IBiliApiServer biliApiServer)
    {
        _biliApiServer = biliApiServer;
    }

    [Route("url")]
    [HttpGet]
    public async ValueTask<MisakaApiResponse<MisakaVideoUrlResponse>> GetVideoUrlByBvid(string bvid = "",
        string avid = "", int page = 0)
    {
        if (string.IsNullOrWhiteSpace(bvid) && string.IsNullOrWhiteSpace(avid))
            throw new ArgumentException("You need at last a bvid or avid");
        if (!string.IsNullOrWhiteSpace(bvid) && !string.IsNullOrWhiteSpace(avid))
            throw new AggregateException("You input avid and bvid at the sametime");

        var useBvid = !string.IsNullOrWhiteSpace(bvid);

        BiliVideoDetail videoDetail;
        videoDetail = useBvid
            ? (await _biliApiServer.GetVideoDetailByBvid(bvid)).Data
            : (await _biliApiServer.GetVideoDetailByAvid(avid)).Data;

        if (page > videoDetail.Pages.Length - 1) throw new ArgumentException("Page out of index", nameof(page));

        var cid = videoDetail.Pages[page].Cid;
        var urlResponse = await _biliApiServer.GetVideoUrlByBvid(videoDetail.Bvid, cid, (int)BiliVideoQuality.R1080P,
            (int)BiliVideoStreamType.Mp4);

        // NO P2P
        var urlInfo = urlResponse.Data.Durl.First();
        var url = "";
        url = urlInfo.Url.Contains("bilivideo.com")
            ? urlInfo.Url
            : urlInfo.BackupUrl.First(url => url.Contains("bilivideo"));

        return new MisakaApiResponse<MisakaVideoUrlResponse>(new MisakaVideoUrlResponse(url, urlResponse.Data.Format,
            urlResponse.Data.Timelength, urlResponse.Data.Quality));
    }

    [Route("url/redirect")]
    [HttpGet]
    public async ValueTask<IActionResult> RedirectToVideoUrl(string bvid = "", string avid = "", int page = 0)
    {
        var urlData = await GetVideoUrlByBvid(bvid, avid, page);
        return Redirect($"/forward/bilibili/{urlData.Data.Url}");
    }
}