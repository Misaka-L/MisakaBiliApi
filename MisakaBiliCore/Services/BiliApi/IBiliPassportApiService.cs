using MisakaBiliCore.Models.BiliApi;
using Refit;

namespace MisakaBiliCore.Services.BiliApi;

public interface IBiliPassportApiService
{
    [Get("/x/passport-login/web/qrcode/generate")]
    public Task<BiliApiResponse<GenerateLoginQrCodeResponse>> GenerateLoginQrCode();

    [Get("/x/passport-login/web/qrcode/poll")]
    public Task<BiliApiResponse<PollLoginQrCodeData>> PollLoginQrCode([AliasAs("qrcode_key")] string qrcodeKey);

    [Post("/x/passport-login/web/cookie/refresh")]
    public Task<BiliApiResponse<BiliRefreshCookiesResponse>> RefreshCookies([AliasAs("csrf")] string csrf,
        [AliasAs("refresh_csrf")] string refreshCsrf,
        [AliasAs("refresh_token")] string refreshToken, [AliasAs("source")] string source = "main_web");

    [Post("/x/passport-login/web/cookie/refresh")]
    public Task<BiliApiResponse> ConfirmRefreshCookies([AliasAs("csrf")] string csrf, [AliasAs("refresh_csrf")] string refreshCsrf);
}
