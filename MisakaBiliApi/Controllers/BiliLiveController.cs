﻿using Microsoft.AspNetCore.Mvc;
using MisakaBiliApi.Models.ApiResponse;
using MisakaBiliCore.Models;
using MisakaBiliCore.Models.BiliApi;
using MisakaBiliCore.Services.BiliApi;

namespace MisakaBiliApi.Controllers;

[ApiController]
[Route("live")]
public class BiliLiveController(IBiliLiveApiService biliLiveApiService) : ControllerBase
{
    [HttpGet("url")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<MisakaVideoUrlResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status302Found)]
    [Produces("application/json")]
    public async Task<IActionResult> GetLiveUrl(string cid, LiveStreamType streamType = LiveStreamType.M3U8,
        bool redirect = false)
    {
        var liveUrlResponse =
            await biliLiveApiService.GetLiveUrlByRoomId(cid, streamType == LiveStreamType.Flv ? "web" : "h5");

        var liveUrl = liveUrlResponse.Data.Durl.First();

        if (redirect)
            return Redirect(liveUrl.Url.ToString());

        return Ok(new MisakaVideoUrlResponse(
            Url: liveUrl.Url.ToString(),
            Format: streamType.ToString(),
            TimeLength: liveUrl.Length,
            Quality: BiliVideoQuality.R1080P
        ));
    }
}