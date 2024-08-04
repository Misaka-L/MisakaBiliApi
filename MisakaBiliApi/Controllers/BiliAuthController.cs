using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MisakaBiliCore.Models;
using MisakaBiliCore.Models.BiliApi;
using MisakaBiliCore.Services;
using MisakaBiliCore.Services.BiliApi;

namespace MisakaBiliApi.Controllers;

[ApiController]
[Route("auth")]
[Authorize(AuthenticationSchemes = "ApiKey", Policy = "ApiKey")]
public class BiliAuthController(
    IBiliApiServices biliApiServices,
    BiliPassportService biliPassportService) : ControllerBase
{
    /// <summary>
    /// 获取已登录的账号的信息
    /// </summary>
    /// <response code="200">返回已登录的账号的信息</response>
    /// <returns>已登录的账号的信息</returns>
    [HttpGet("user")]
    [ProducesResponseType<BiliUser>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<BiliUser> GetLoginUser()
    {
        return (await biliApiServices.GetLoginUserInfo()).Data;
    }

    /// <summary>
    /// 获取登录二维码
    /// </summary>
    /// <response code="200">返回登录二维码信息</response>
    /// <returns>登录二维码信息</returns>
    [HttpPost("login/qr-code")]
    [ProducesResponseType<BiliUser>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<LoginQrCodeData> GetLoginQrCode()
    {
        var qrCodeData = await biliPassportService.GenerateLoginQrCodeAsync();

        return qrCodeData;
    }

    /// <summary>
    /// 刷新登录二维码状态和完成二维码登录
    /// </summary>
    /// <response code="200">返回登录二维码状态</response>
    /// <returns>登录二维码状态</returns>
    [HttpPost("login/qr-code/poll")]
    [ProducesResponseType<BiliUser>(StatusCodes.Status200OK)]
    [Produces("application/json")]
    public async Task<PollLoginQrCodeData> PollLoginQrCode([FromQuery] string qrCodeKey)
    {
        return await biliPassportService.GetQrCodeLoginStatusAsync(qrCodeKey);
    }
}
