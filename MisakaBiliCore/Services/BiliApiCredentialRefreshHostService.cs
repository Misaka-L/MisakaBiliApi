using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MisakaBiliCore.Services.BiliApi;

namespace MisakaBiliCore.Services;

public class BiliApiCredentialRefreshHostService(
    BiliPassportService biliPassportService,
    IBiliApiServices biliApiServices,
    IBiliPassportApiService biliPassportApiService,
    BiliApiSecretStorageService biliApiSecretStorageService,
    ILogger<BiliApiCredentialRefreshHostService> logger) : IHostedService
{
    private readonly CancellationTokenSource _refreshWbiCancellationTokenSource = new();
    private readonly CancellationTokenSource _refreshBiliTicketCancellationTokenSource = new();
    private CancellationTokenSource? _refreshCookiesCancellationTokenSource = new();

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await biliApiSecretStorageService.LoadSecrets();

        _ = RefreshBiliTicketLoop();
        _ = RefreshWbiKeyLoop();

        biliPassportService.Login += async (_, _) =>
        {
            if (_refreshCookiesCancellationTokenSource is not null)
            {
                await _refreshCookiesCancellationTokenSource.CancelAsync();
                _refreshCookiesCancellationTokenSource.Dispose();
            }

            _ = RefreshCookieLoop();
        };

        // Is user login?
        try
        {
            await biliApiServices.GetLoginUserInfo();
        }
        catch
        {
            return;
        }

        _ = RefreshCookieLoop();
    }

    private async Task RefreshWbiKeyLoop()
    {
        while (!_refreshWbiCancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Start Refresh Wbi Key");
                await biliPassportService.GetWbiKeysAsync();
                logger.LogInformation("Refresh Wbi Key Success");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to refresh wbi key");
            }

            await Task.Delay(TimeSpan.FromHours(1));
        }
    }

    private async Task RefreshBiliTicketLoop()
    {
        while (!_refreshBiliTicketCancellationTokenSource.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation("Start Refresh Bili Ticket");
                var biliTicket = await biliPassportService.GetBiliTicketAsync();
                logger.LogInformation("Refresh Bili Ticket Success, Ttl: {Ttl}", biliTicket.Ttl);

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(biliTicket.Ttl) - TimeSpan.FromMinutes(1),
                        _refreshBiliTicketCancellationTokenSource.Token);
                }
                catch (TaskCanceledException)
                {
                    // ignore
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to refresh bili ticket");
                await Task.Delay(TimeSpan.FromMinutes(1), _refreshBiliTicketCancellationTokenSource.Token);
            }
        }
    }

    private async Task RefreshCookieLoop()
    {
        _refreshCookiesCancellationTokenSource = new CancellationTokenSource();

        while (!_refreshCookiesCancellationTokenSource.IsCancellationRequested)
        {
            await RefreshCookie();
            await Task.Delay(TimeSpan.FromHours(1), _refreshCookiesCancellationTokenSource.Token);
        }
    }

    private async Task RefreshCookie()
    {
        try
        {
            var biliJct = biliApiSecretStorageService.CookieContainer.GetAllCookies()
                .FirstOrDefault(cookie => cookie.Name == "bili_jct")?.Value;

            logger.LogInformation("Check Cookie Status");
            var cookieStatusResponse = await biliPassportApiService.IsCookieNeedRefresh(biliJct);
            if (!cookieStatusResponse.Data.NeedRefresh)
            {
                logger.LogInformation("Cookie is not need to refresh");
                return;
            }

            logger.LogInformation("Start Refresh Cookies");
            await biliPassportService.RefreshCookiesAsync();
            logger.LogInformation("Refresh Cookies Success");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to refresh cookies");
        }
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        await _refreshBiliTicketCancellationTokenSource.CancelAsync();

        if (_refreshCookiesCancellationTokenSource is not null)
            await _refreshCookiesCancellationTokenSource.CancelAsync();

        _refreshBiliTicketCancellationTokenSource.Dispose();
        _refreshCookiesCancellationTokenSource?.Dispose();
    }
}
