using MisakaBiliCore.Services;

namespace MisakaBiliCore;

public class BiliTicketRequestHandler(BiliPassportService biliPassportService) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        await biliPassportService.GetBiliTicketAsync();

        return await base.SendAsync(request, cancellationToken);
    }
}
