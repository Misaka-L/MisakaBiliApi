using Microsoft.AspNetCore.Mvc;
using MisakaBiliCore.Models;
using MisakaBiliCore.Models.BiliApi;
using MisakaBiliCore.Services;
using MisakaBiliCore.Services.BiliApi;

namespace MisakaBiliApi.Controllers;

[ApiController]
[Route("auth")]
public class BiliAuthController(
    IBiliApiServices biliApiServices,
    BiliPassportService biliPassportService) : ControllerBase
{
    [HttpGet("user")]
    public async Task<BiliUser> GetLoginUser()
    {
        return (await biliApiServices.GetLoginUserInfo()).Data;
    }

    [HttpPost("login/qr-code")]
    public async Task<LoginQrCodeData> GetLoginQrCode()
    {
        var qrCodeData = await biliPassportService.GenerateLoginQrCodeAsync();

        return qrCodeData;
    }

    [HttpPost("login/qr-code/poll")]
    public async Task<PollLoginQrCodeData> PollLoginQrCode([FromQuery] string qrCodeKey)
    {
        return await biliPassportService.GetQrCodeLoginStatusAsync(qrCodeKey);
    }
}
