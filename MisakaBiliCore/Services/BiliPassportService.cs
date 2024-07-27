using System.Net;
using System.Net.Http.Json;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
using MisakaBiliCore.Models;
using MisakaBiliCore.Models.BiliApi;
using MisakaBiliCore.Services.BiliApi;
using MisakaBiliCore.Utils;

namespace MisakaBiliCore.Services;

public partial class BiliPassportService(
    IBiliPassportApiService biliPassportApiService,
    IHttpClientFactory httpClientFactory,
    BiliApiSecretStorageService biliApiSecretStorageService,
    ILogger<BiliPassportService> logger)
{
    [GeneratedRegex("""<div id="1-name">(([\s\S])*?)<\/div>""")]
    private static partial Regex CorrenspondRegex();

    public async Task<LoginQrCodeData> GenerateLoginQrCodeAsync()
    {
        logger.LogInformation("Start Generate Login QrCode");

        await GetBiliTicketAsync();

        var loginQrCodeResponse = await biliPassportApiService.GenerateLoginQrCode();

        logger.LogInformation("Generate Login QrCode Success, Url: {Url}, QrCodeKey: {QrCodeKey}",
            loginQrCodeResponse.Data.Url, loginQrCodeResponse.Data.QrCodeKey);

        return new LoginQrCodeData(
            Url: loginQrCodeResponse.Data.Url,
            QrCodeKey: loginQrCodeResponse.Data.QrCodeKey
        );
    }

    public async Task StartAutoQrCodeLoginProcessAsync(string qrCodeKey, CancellationToken? cancellationToken = null)
    {
        while (cancellationToken?.IsCancellationRequested ?? true)
        {
            var status = await GetQrCodeLoginStatusAsync(qrCodeKey);

            if (status.Code == QrCodeLoginStatus.QrCodeExpired)
            {
                logger.LogError("QrCode {Key} Expired", qrCodeKey);
                throw new InvalidOperationException("QrCode Expired");
            }

            await Task.Delay(TimeSpan.FromSeconds(1));
        }
    }

    public async Task<PollLoginQrCodeData> GetQrCodeLoginStatusAsync(string qrCodeKey)
    {
        logger.LogInformation("Start Poll Login QrCode Status, QrCodeKey: {QrCodeKey}", qrCodeKey);

        var pollResponse = await biliPassportApiService.PollLoginQrCode(qrCodeKey);

        logger.LogInformation("QrCode {Key} Status: {StatusCode}", qrCodeKey, pollResponse.Data.Code);

        if (pollResponse.Data.Code != QrCodeLoginStatus.Success)
        {
            await GetBiliTicketAsync();

            return pollResponse.Data;
        }

        biliApiSecretStorageService.RefreshToken = pollResponse.Data.RefreshToken;

        logger.LogInformation("QrCode Login success");

        await biliApiSecretStorageService.SaveSecrets();

        return pollResponse.Data;
    }

    public async Task RefreshCookiesAsync()
    {
        using var httpClient = httpClientFactory.CreateClient("biliMainWeb");

        logger.LogInformation("Start Refresh Cookies");

        var correnspondPath = "https://www.bilibili.com/correspond/1/" +
                              CorrespondPathUtils.GetCorrespondPath(DateTimeOffset.Now - TimeSpan.FromMinutes(1));

        logger.LogDebug("Correspond Path: {Path}", correnspondPath);

        var correspondPageHtml = await httpClient.GetStringAsync(correnspondPath);

        logger.LogDebug("Correspond Page Html:\n{Html}", correspondPageHtml);

        var responseHtmlMatch = CorrenspondRegex().Match(correspondPageHtml);
        var refreshCsrf = responseHtmlMatch.Groups[1].Value;

        logger.LogDebug("Refresh CSRF: {RefreshCsrf}", refreshCsrf);

        await GetBiliTicketAsync();

        var refreshResponse = await biliPassportApiService.RefreshCookies(
            biliApiSecretStorageService.CookieContainer.GetAllCookies().First(cookie => cookie.Name == "bili_jct")
                .Value,
            refreshCsrf, biliApiSecretStorageService.RefreshToken);

        biliApiSecretStorageService.RefreshToken = refreshResponse.Data.RefreshToken;

        await biliApiSecretStorageService.SaveSecrets();

        logger.LogInformation("Refresh Cookies Success, Confirm Refresh Cookies");

        await biliPassportApiService.ConfirmRefreshCookies(
            biliApiSecretStorageService.CookieContainer.GetAllCookies().First(cookie => cookie.Name == "bili_jct")
                .Value, biliApiSecretStorageService.RefreshToken);

        logger.LogInformation("Confirm Refresh Cookies Success");
    }

    public async Task<BiliTicketResponse> GetBiliTicketAsync()
    {
        using var httpClient = httpClientFactory.CreateClient("biliapi");

        logger.LogInformation("Start Get BiliTicket");

        var dateTime = TimeProvider.System.GetUtcNow();
        var message = BiliTicketUtils.GetBiliTicketHexSign(dateTime);

        logger.LogDebug("BiliTicket HexSign: {HexSign}", message);

        var queryString = await new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            { "hexsign", message },
            { "context[ts]", dateTime.ToUnixTimeSeconds().ToString() },
            { "key_id", "ec02" },
            { "csrf", biliApiSecretStorageService.CookieContainer.GetAllCookies().FirstOrDefault(cookie => cookie.Name == "bili_jct")?.Value ?? "" }
        }).ReadAsStringAsync();

        logger.LogInformation("Getting BiliTicket...");

        var biliTicketHttpResponse =
            await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post,
                "/bapis/bilibili.api.ticket.v1.Ticket/GenWebTicket?" + queryString));

        var biliTicketResponse =
            await biliTicketHttpResponse.Content.ReadFromJsonAsync<BiliApiResponse<BiliTicketResponse>>();

        var expiresAt =
            DateTimeOffset.FromUnixTimeSeconds(biliTicketResponse.Data.CreatedAt) +
            TimeSpan.FromSeconds(biliTicketResponse.Data.Ttl);

        biliApiSecretStorageService.CookieContainer.Add(new Cookie("bili_ticket", biliTicketResponse.Data.Ticket, "/", ".bilibili.com")
        {
            Expires = expiresAt.DateTime
        });
        biliApiSecretStorageService.CookieContainer.Add(new Cookie("bili_ticket_expires", expiresAt.ToUnixTimeSeconds().ToString(), "/",
            ".bilibili.com")
        {
            Expires = expiresAt.DateTime
        });

        await biliApiSecretStorageService.SaveSecrets();

        logger.LogDebug("BiliTicket: {Ticket}", biliTicketResponse.Data.Ticket);
        logger.LogInformation("Get BiliTicket Success, It will expires at: {ExpiresAt}", expiresAt);

        return biliTicketResponse.Data;
    }
}
