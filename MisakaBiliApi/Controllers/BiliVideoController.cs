using Microsoft.AspNetCore.Mvc;
using MisakaBiliApi.Models.ApiResponse;
using MisakaBiliCore.Models.BiliApi;
using MisakaBiliCore.Services.BiliApi;
using MisakaBiliCore.Utils;

namespace MisakaBiliApi.Controllers;

/// <summary>
/// 哔哩哔哩视频 Api 控制器
/// </summary>
[ApiController]
[Route("video")]
public class BiliVideoController(IBiliApiServices biliApiServices) : ControllerBase
{
    /// <summary>
    /// 请求哔哩哔哩视频流地址 (MP4)
    /// </summary>
    /// <param name="bvid">BV 号</param>
    /// <param name="avid">AV 号（纯数字）</param>
    /// <param name="page">分 P（从 0 开始）</param>
    /// <param name="redirect">是否直接重定向到视频流 URL</param>
    /// <returns>返回或重定向到 MP4 视频流地址</returns>
    /// <remarks>
    /// 示例请求（BV 号）:
    ///
    ///     GET /video/url/mp4?bvid=BV1LP411v7Bv
    ///     GET /video/url/mp4?bvid=BV1LP411v7Bv&amp;redirect=true (获取视频流 URL 并重定向)
    ///     GET /video/url/mp4?bvid=BV1mx411M793&amp;page=2 (获取 P3 的视频流 URL)
    ///
    /// 示例请求（AV 号）:
    ///
    ///     GET /video/url/mp4?avid=315594987
    ///     GET /video/url/mp4?avid=315594987&amp;redirect=true (获取视频流 URL 并重定向)
    ///     GET /video/url/mp4?bvid=15627712&amp;page=2 (获取 P3 的视频流 URL)
    ///
    /// 示例响应:
    ///
    ///     {
    ///        "url": "https://*.bilivideo.com/*",
    ///        "format": "mp4720",
    ///        "timeLength": 222000,
    ///        "quality": 64
    ///     }
    /// </remarks>
    /// <response code="400">请求参数错误</response>
    /// <response code="200">返回视频流地址</response>
    /// <response code="302">重定向到视频流地址</response>
    [HttpGet("url/mp4")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<MisakaVideoStreamUrlResponse>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async ValueTask<ActionResult<MisakaVideoStreamUrlResponse>> GetVideoUrl(string bvid = "",
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

        var useBvid = !string.IsNullOrWhiteSpace(bvid);

        try
        {
            var pageDetail = await GetVideoPageInternal(bvid, avid, page);

            var urlResponse = useBvid
                ? await biliApiServices.GetVideoUrlByBvid(bvid, pageDetail.Cid, (int)BiliVideoQuality.R1080PHighRate,
                    (int)BiliVideoStreamType.Mp4)
                : await biliApiServices.GetVideoUrlByAvid(avid, pageDetail.Cid, (int)BiliVideoQuality.R1080PHighRate,
                    (int)BiliVideoStreamType.Mp4);

            var urls = urlResponse.Data.Durl.SelectMany(durl => (string[]) [durl.Url, ..durl.BackupUrl ?? []])
                .ToArray();
            var url = NoP2PUtils.GetNoP2PUrl(urls);

            if (redirect)
            {
                return Redirect(url);
            }

            return new MisakaVideoStreamUrlResponse(
                Url: url,
                Format: urlResponse.Data.Format,
                TimeLength: urlResponse.Data.Timelength,
                Quality: urlResponse.Data.Quality
            );
        }
        catch (ArgumentException argumentException)
        {
            ModelState.AddModelError(argumentException.ParamName ?? "", argumentException.Message);
            return BadRequest();
        }
    }

    private async ValueTask<BiliVideoPage> GetVideoPageInternal(string? bvid = null, string? avid = null, int page = 0)
    {
        if (string.IsNullOrWhiteSpace(bvid) && string.IsNullOrWhiteSpace(avid))
            throw new ArgumentException("You need at last a bvid or avid");
        if (!string.IsNullOrWhiteSpace(bvid) && !string.IsNullOrWhiteSpace(avid))
            throw new ArgumentException("You input avid and bvid at the sametime");

        var useBvid = !string.IsNullOrWhiteSpace(bvid);

        var videoDetail = useBvid
            ? (await biliApiServices.GetVideoDetailByBvid(bvid)).Data
            : (await biliApiServices.GetVideoDetailByAid(avid)).Data;

        if (page > videoDetail.Pages.Length - 1) throw new ArgumentOutOfRangeException(nameof(page), "Page out of index");

        return videoDetail.Pages[page];
    }
}
