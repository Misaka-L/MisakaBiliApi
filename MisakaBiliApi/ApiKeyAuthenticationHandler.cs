using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace MisakaBiliApi;

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<ApiKeyAuthenticationOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder)
    : AuthenticationHandler<ApiKeyAuthenticationOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authHeader = Request.Headers["X-Api-Key"];

        if (string.IsNullOrWhiteSpace(authHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        if (authHeader != Options.ApiKey)
        {
            Task.FromResult(AuthenticateResult.Fail("Invalid ApiKey"));
        }

        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(new[]
        {
            new ClaimsIdentity(new[]
            {
                new Claim("ApiKey", authHeader.ToString())
            })
        }), Scheme.Name)));
    }
}

public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
{
    public string ApiKey { get; set; }
}
