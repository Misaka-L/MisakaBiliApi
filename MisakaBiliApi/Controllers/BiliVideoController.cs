using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using MisakaBiliApi.Models.ApiResponse;
using MisakaBiliCore.Models.BiliApi;
using MisakaBiliCore.Services;

namespace MisakaBiliApi.Controllers;

/// <summary>
/// 哔哩哔哩视频 Api 控制器
/// </summary>
[Route("/api/bilibili/video/")]
public partial class BiliVideoController : Controller
{
    private readonly IBiliApiServer _biliApiServer;
    private readonly ILogger<BiliVideoController> _logger;

    public BiliVideoController(IBiliApiServer biliApiServer, ILogger<BiliVideoController> logger)
    {
        _biliApiServer = biliApiServer;
        _logger = logger;
    }

    /// <summary>
    /// 请求哔哩哔哩视频流地址 (MP4)
    /// </summary>
    /// <param name="bvid">BV 号</param>
    /// <param name="avid">AV 号（纯数字）</param>
    /// <param name="page">分 P（从 0 开始）</param>
    /// <param name="redirect">是否直接重定向到视频 URL</param>
    /// <returns>返回 MP4 视频流地址</returns>
    /// <remarks>
    /// 示例请求（BV 号）:
    ///
    ///     GET /api/bilibili/video/url/mp4?bvid=BV1LP411v7Bv
    ///     GET /api/bilibili/video/url/mp4?bvid=BV1mx411M793&amp;page=2 (获取 P3 的视频链接)
    ///
    /// 示例请求（AV 号）:
    ///
    ///     GET /api/bilibili/video/url/mp4?avid=315594987
    ///     GET /api/bilibili/video/url/mp4?bvid=15627712&amp;page=2 (获取 P3 的视频链接)
    ///
    /// 示例响应:
    ///
    ///     {
    ///        "data": {
    ///            "url": "https://*.bilivideo.com/*",
    ///               "format": "mp4720",
    ///               "timelength": 222000,
    ///               "quality": 64
    ///        },
    ///        "message": "",
    ///        "code": 200
    ///     }
    /// </remarks>
    /// <response code="400">请求参数错误</response>
    /// <response code="200">返回视频流地址</response>
    [HttpGet("url/mp4")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<MisakaVideoUrlResponse>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async ValueTask<ActionResult<MisakaVideoUrlResponse>> GetVideoUrl(string bvid = "",
        string avid = "", int page = 0, bool redirect = false)
    {
        if (string.IsNullOrWhiteSpace(bvid) && string.IsNullOrWhiteSpace(avid))
        {
            ModelState.AddModelError(nameof(bvid), "You need at last a bvid or avid");
            ModelState.AddModelError(nameof(avid), "You need at last a bvid or avid");
        }

        if (!string.IsNullOrWhiteSpace(bvid) && !string.IsNullOrWhiteSpace(avid))
        {
            ModelState.AddModelError(nameof(bvid), "You input avid and bvid at the sametime");
            ModelState.AddModelError(nameof(avid), "You input avid and bvid at the sametime");
        }

        if (!ModelState.IsValid) return BadRequest();

        var response = await GetVideoUrlInternal(bvid, avid, page);

        if (redirect)
        {
            return Redirect(response.Url);
        }

        return response;
    }

    /// <summary>
    /// 重定向到视频流反向代理地址
    /// </summary>
    /// <param name="bvid">BV 号</param>
    /// <param name="avid">AV 号（纯数字）</param>
    /// <param name="page">分 P（从 0 开始）</param>
    /// <returns>重定向到视频流反向代理地址</returns>
    /// <remarks>
    /// 示例请求（BV 号）:
    ///
    ///     GET /api/bilibili/video/url/mp4/redirect?bvid=BV1LP411v7Bv
    ///     GET /api/bilibili/video/url/mp4/redirect?bvid=BV1mx411M793&amp;page=2 (获取 P3 的视频链接)
    ///
    /// 示例请求（AV 号）:
    ///
    ///     GET /api/bilibili/video/url/mp4/redirect?avid=315594987
    ///     GET /api/bilibili/video/url/mp4/redirect?bvid=15627712&amp;page=2 (获取 P3 的视频链接)
    /// </remarks>
    /// <response code="400">请求参数错误</response>
    /// <response code="302">重定向视频流反向代理地址</response>
    [Route("url/mp4/proxy")]
    [HttpGet]
    [ProducesResponseType<MisakaVideoUrlResponse>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    public async ValueTask<IActionResult> RedirectToVideoUrl(string bvid = "", string avid = "", int page = 0)
    {
        if (string.IsNullOrWhiteSpace(bvid) && string.IsNullOrWhiteSpace(avid))
        {
            ModelState.AddModelError(nameof(bvid), "You need at last a bvid or avid");
            ModelState.AddModelError(nameof(avid), "You need at last a bvid or avid");
        }

        if (!string.IsNullOrWhiteSpace(bvid) && !string.IsNullOrWhiteSpace(avid))
        {
            ModelState.AddModelError(nameof(bvid), "You input avid and bvid at the sametime");
            ModelState.AddModelError(nameof(avid), "You input avid and bvid at the sametime");
        }

        if (!ModelState.IsValid) return BadRequest();

        var urlData = await GetVideoUrlInternal(bvid, avid, page);
        return Redirect($"/forward/bilibili/{urlData.Url.Replace("https://", "")}");
    }

    [GeneratedRegex("(mcdn.bilivideo.(cn|com)|szbdyd.com)")]
    private static partial Regex P2PRegex();

    private async ValueTask<MisakaVideoUrlResponse> GetVideoUrlInternal(string bvid = "",
        string avid = "", int page = 0)
    {
        if (string.IsNullOrWhiteSpace(bvid) && string.IsNullOrWhiteSpace(avid))
            throw new ArgumentException("You need at last a bvid or avid");
        if (!string.IsNullOrWhiteSpace(bvid) && !string.IsNullOrWhiteSpace(avid))
            throw new ArgumentException("You input avid and bvid at the sametime");

        var useBvid = !string.IsNullOrWhiteSpace(bvid);

        BiliVideoDetail videoDetail;
        videoDetail = useBvid
            ? (await _biliApiServer.GetVideoDetailByBvid(bvid)).Data
            : (await _biliApiServer.GetVideoDetailByAid(avid)).Data;

        if (page > videoDetail.Pages.Length - 1) throw new ArgumentException("Page out of index", nameof(page));

        var cid = videoDetail.Pages[page].Cid;
        var urlResponse = await _biliApiServer.GetVideoUrlByBvid(videoDetail.Bvid, cid, (int)BiliVideoQuality.R1080P,
            (int)BiliVideoStreamType.Mp4);

        _logger.LogInformation("BiliBili Api Response: {Response}", JsonSerializer.Serialize(urlResponse, new JsonSerializerOptions()
        {
            WriteIndented = true
        }));

        // NO P2P
        var p2pRegex = P2PRegex();
        var urlInfo = urlResponse.Data.Durl.First();
        string[] urls = [urlInfo.Url, ..urlInfo.BackupUrl ?? []];
        var url = urls.First(url => !p2pRegex.IsMatch(url));

        return new MisakaVideoUrlResponse(url, urlResponse.Data.Format,
            urlResponse.Data.Timelength, urlResponse.Data.Quality);
    }
}
