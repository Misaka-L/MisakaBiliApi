using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MisakaBiliApi.Models.ApiResponse;
using MisakaBiliCore.Models;
using MisakaBiliCore.Services.BiliApi;

namespace MisakaBiliApi.Controllers;

/// <summary>
/// 哔哩哔哩直播 Api 控制器
/// </summary>
[ApiController]
[Route("live")]
[OutputCache(PolicyName = "LiveStreamUrlCache")]
public class BiliLiveController(IBiliLiveApiService biliLiveApiService) : ControllerBase
{
    /// <summary>
    /// 请求哔哩哔哩直播流地址
    /// </summary>
    /// <param name="roomId">直播间 ID</param>
    /// <param name="streamType">直播流类型</param>
    /// <param name="redirect">是否重定向到直播流 URL</param>
    /// <remarks>
    /// 示例请求:
    ///
    ///     GET /live/url?roomId=6
    ///     GET /live/url?roomId=6&amp;redirect=true (获取直播流 URL 并重定向)
    ///
    /// 示例响应:
    ///
    ///     {
    ///        "url": "https://*.bilivideo.com/*",
    ///        "quality": "4",
    ///        "streamType": 0,
    ///        "quality": 64
    ///     }
    /// </remarks>
    /// <returns>返回或重定向到直播流地址</returns>
    /// <response code="400">请求参数错误</response>
    /// <response code="200">返回直播流地址</response>
    /// <response code="302">重定向到直播流地址</response>
    [HttpGet("url")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<MisakaLiveStreamUrlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [Produces("application/json")]
    public async Task<IActionResult> GetLiveUrl(string roomId, LiveStreamType streamType = LiveStreamType.M3U8,
        bool redirect = false)
    {
        var liveUrlResponse =
            await biliLiveApiService.GetLiveUrlByRoomId(roomId, streamType == LiveStreamType.Flv ? "web" : "h5");

        var liveUrl = liveUrlResponse.Data.Durl.First();

        if (redirect)
            return Redirect(liveUrl.Url.ToString());

        return Ok(new MisakaLiveStreamUrlResponse(
            Url: liveUrl.Url.ToString(),
            Quality: liveUrlResponse.Data.CurrentQuality,
            StreamType: streamType
        ));
    }
}
