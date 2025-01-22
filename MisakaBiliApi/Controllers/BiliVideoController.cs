using Microsoft.AspNetCore.Mvc;
using MisakaBiliApi.Models;
using MisakaBiliApi.Models.ApiResponse;
using MisakaBiliCore.Models.BiliApi;
using MisakaBiliCore.Services;

namespace MisakaBiliApi.Controllers;

/// <summary>
/// 哔哩哔哩视频 Api 控制器
/// </summary>
[ApiController]
[Route("video")]
public class BiliVideoController(
    BiliStreamUrlRequestService biliStreamUrlRequestService) : ControllerBase
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
    /// 示例请求（AV 号）:
    ///
    ///     GET /video/url/mp4?avid=315594987
    ///     GET /video/url/mp4?avid=315594987&amp;redirect=true (获取视频流 URL 并重定向)
    ///     GET /video/url/mp4?avid=15627712&amp;page=2 (获取 P3 的视频流 URL)
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
    [ProducesResponseType<MisakaVideoStreamMp4UrlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [Produces("application/json")]
    public async ValueTask<ActionResult<MisakaVideoStreamMp4UrlResponse>> GetVideoMp4Url(string bvid = "",
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
            var urlResponse = await biliStreamUrlRequestService.GetVideoMp4StreamUrlAsync(useBvid ? bvid : avid, page, BiliVideoQuality.R1080PHighRate);
            var url = urlResponse.Durl[0].Url;

            if (redirect)
            {
                return Redirect(url);
            }

            return new MisakaVideoStreamMp4UrlResponse(
                Url: url,
                Format: urlResponse.Format,
                TimeLength: urlResponse.Timelength,
                Quality: urlResponse.Quality
            );
        }
        catch (ArgumentException argumentException)
        {
            ModelState.AddModelError(argumentException.ParamName ?? "", argumentException.Message);
            return BadRequest();
        }
    }

    /// <summary>
    /// 请求哔哩哔哩视频流地址 (DASH)
    /// </summary>
    /// <param name="bvid">BV 号</param>
    /// <param name="avid">AV 号（纯数字）</param>
    /// <param name="page">分 P（从 0 开始）</param>
    /// <param name="redirect">重定向到视频流或音频流 URL或不重定向</param>
    /// <returns>返回或重定向到 DASH 视频流地址</returns>
    /// <remarks>
    /// 示例请求（BV 号）:
    ///
    ///     GET /video/url/dash?bvid=BV1LP411v7Bv
    ///     GET /video/url/dash?bvid=BV1LP411v7Bv&amp;redirect=2 (获取并重定向音频流 URL)
    ///     GET /video/url/dash?bvid=BV1mx411M793&amp;page=2 (获取 P3 的 DASH 流)
    /// 示例请求（AV 号）:
    ///
    ///     GET /video/url/dash?avid=315594987
    ///     GET /video/url/dash?avid=315594987&amp;redirect=2 (获取并重定向音频流 URL)
    ///     GET /video/url/dash?avid=15627712&amp;page=2 (获取 P3 的 DASH 流)
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
    [HttpGet("url/dash")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<MisakaVideoStreamDashUrlResponse>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async ValueTask<ActionResult<MisakaVideoStreamDashUrlResponse>> GetVideoDashUrl(string bvid = "",
        string avid = "", int page = 0, DashRequestRedirectType redirect = DashRequestRedirectType.None)
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
            var urlResponse = useBvid ? await biliStreamUrlRequestService.GetVideoDashStreamUrlAsync(bvid, page)
                : await biliStreamUrlRequestService.GetVideoDashStreamUrlAsync(avid, page);

            var videoDashs = GetDashs(urlResponse.Dash.Video);
            if (redirect == DashRequestRedirectType.Video)
            {
                return Redirect(videoDashs[0].Urls[0]);
            }

            var audioDashs = GetDashs(urlResponse.Dash.Audio);
            if (redirect == DashRequestRedirectType.Audio)
            {
                return Redirect(audioDashs[0].Urls[0]);
            }

            return new MisakaVideoStreamDashUrlResponse(
                VideoDashs: videoDashs,
                AudioDashs: audioDashs,
                Duration: urlResponse.Dash.Duration
            );
        }
        catch (ArgumentException argumentException)
        {
            ModelState.AddModelError(argumentException.ParamName ?? "", argumentException.Message);
            return BadRequest();
        }
    }

    private static MisakaVideoDashItem[] GetDashs(BiliVideoDashUrlItem[] dashs)
    {
        return dashs.Select(dash =>
            {
                return new MisakaVideoDashItem(
                    Urls: [dash.BaseUrl.ToString(), ..dash.BackupUrls?.Select(url => url.ToString()) ?? []],
                    FrameRate: dash.FrameRate,
                    Width: dash.Width,
                    Height: dash.Height,
                    Codecs: dash.Codecs,
                    Bandwidth: dash.Bandwidth,
                    Id: dash.Id
                );
            })
            .OrderByDescending(dash => dash.Bandwidth)
            .ToArray();
    }
}
