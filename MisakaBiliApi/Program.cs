using System.Net;
using System.Reflection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using MisakaBiliApi;
using MisakaBiliApi.Options;
using MisakaBiliCore;
using MisakaBiliCore.Options;
using MisakaBiliCore.Services;
using MisakaBiliCore.Services.BiliApi;
using Refit;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;

Dictionary<string, string> defaultRequestHeader = new()
{
    { "Connection", "keep-alive" },
    { "sec-ch-ua", "\"Not)A;Brand\";v=\"99\", \"Microsoft Edge\";v=\"127\", \"Chromium\";v=\"127\"" },
    { "sec-ch-ua-mobile", "?0" },
    { "sec-ch-ua-platform", "\"Windows\"" },
    { "DNT", "1" },
    { "Upgrade-Insecure-Requests", "1" },
    {
        "User-Agent",
        "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/127.0.0.0 Safari/537.36 Edg/127.0.0.0"
    },
    {
        "Accept",
        "text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.7"
    },
    { "Sec-Fetch-Site", "none" },
    { "Sec-Fetch-Mode", "navigate" },
    { "Sec-Fetch-User", "?1" },
    { "Sec-Fetch-Dest", "document" },
    { "Accept-Encoding", "gzip, deflate, br, zstd" },
    { "Accept-Language", "zh-CN,zh;q=0.9" }
};

var logTemplate =
    "[{@t:yyyy-MM-dd HH:mm:ss} {@l:u3}] [{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {@m}\n{@x}";

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.File(new ExpressionTemplate(logTemplate), "logs/app-.log", rollingInterval: RollingInterval.Day)
    .WriteTo.Console(new ExpressionTemplate(logTemplate, theme: TemplateTheme.Literate))
    .WriteTo.Debug(new ExpressionTemplate(logTemplate))
    .CreateLogger();

#region ApiDoc

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "Misaka-L's Bili Api",
        Description = "一个简单的哔哩哔哩 API",
        Contact = new OpenApiContact
        {
            Name = "Misaka-L",
            Email = "lipww1234@foxmail.com",
            Url = new Uri("https://misakal.xyz"),
            Extensions = new Dictionary<string, IOpenApiExtension>()
            {
                { "BiliBili", new OpenApiString("https://space.bilibili.com/64514574") }
            }
        }
    });

    options.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Name = "X-Api-Key",
        Type = SecuritySchemeType.ApiKey
    });

    options.OperationFilter<SecurityRequirementsOperationFilter>();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

#endregion

#region Options

builder.Services.AddOptions<ApiBaseUrlOptions>()
    .Bind(builder.Configuration.GetSection("ApiBaseUrl"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<ManagementApiKeyOptions>()
    .Bind(builder.Configuration.GetSection("ManagementApiKey"))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var managementApiKeyOptions = builder.Configuration.GetSection("ManagementApiKey").Get<ManagementApiKeyOptions>() ??
                              new ManagementApiKeyOptions();

#endregion

builder.Services.AddAuthentication()
    .AddScheme<ApiKeyAuthenticationOptions, ApiKeyAuthenticationHandler>("ApiKey",
        options => { options.ApiKey = managementApiKeyOptions.ApiKey; });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ApiKey", policy => { policy.RequireClaim("ApiKey", managementApiKeyOptions.ApiKey); });
});

builder.Services.AddSingleton<BiliApiSecretStorageService>();
builder.Services.AddSingleton<BiliPassportService>();

builder.Services.AddTransient<WbiRequestHandler>();

builder.Services.AddHostedService<BiliApiCredentialRefreshHostService>();

#region HttpClient & Refit

#region HttpClient

builder.Services.AddHttpClient("biliapi", (services, client) =>
{
    var options = services.GetRequiredService<IOptions<ApiBaseUrlOptions>>().Value;

    foreach (var (headerName, value) in defaultRequestHeader)
    {
        client.DefaultRequestHeaders.Add(headerName, value);
    }

    client.BaseAddress = new Uri(options.BiliApiBaseUrl);
}).ConfigurePrimaryHttpMessageHandler(services => new SocketsHttpHandler
{
    AutomaticDecompression = DecompressionMethods.All,
    CookieContainer = services.GetRequiredService<BiliApiSecretStorageService>().CookieContainer
});

builder.Services.AddHttpClient("biliMainWeb", (services, client) =>
{
    var options = services.GetRequiredService<IOptions<ApiBaseUrlOptions>>().Value;

    foreach (var (headerName, value) in defaultRequestHeader)
    {
        client.DefaultRequestHeaders.Add(headerName, value);
    }

    client.BaseAddress = new Uri(options.BiliWebBaseUrl);
}).ConfigurePrimaryHttpMessageHandler(services => new SocketsHttpHandler
{
    AutomaticDecompression = DecompressionMethods.All,
    CookieContainer = services.GetRequiredService<BiliApiSecretStorageService>().CookieContainer
});

#endregion

builder.Services.AddRefitClient<IBiliApiServices>(null, httpClientName: "biliapi")
    .AddHttpMessageHandler<WbiRequestHandler>();

builder.Services.AddRefitClient<IBiliLiveApiService>()
    .ConfigureHttpClient((services, client) =>
    {
        var options = services.GetRequiredService<IOptions<ApiBaseUrlOptions>>().Value;

        foreach (var (headerName, value) in defaultRequestHeader)
        {
            client.DefaultRequestHeaders.Add(headerName, value);
        }

        client.BaseAddress = new Uri(options.BiliLiveApiBaseUrl);
    }).ConfigurePrimaryHttpMessageHandler(services => new SocketsHttpHandler
    {
        AutomaticDecompression = DecompressionMethods.All,
        CookieContainer = services.GetRequiredService<BiliApiSecretStorageService>().CookieContainer
    });

builder.Services.AddRefitClient<IBiliPassportApiService>()
    .ConfigureHttpClient((services, client) =>
    {
        var options = services.GetRequiredService<IOptions<ApiBaseUrlOptions>>().Value;

        foreach (var (headerName, value) in defaultRequestHeader)
        {
            client.DefaultRequestHeaders.Add(headerName, value);
        }

        client.BaseAddress = new Uri(options.BiliPassportBaseUrl);
    }).ConfigurePrimaryHttpMessageHandler(services => new SocketsHttpHandler
    {
        AutomaticDecompression = DecompressionMethods.All,
        CookieContainer = services.GetRequiredService<BiliApiSecretStorageService>().CookieContainer
    });

#endregion

builder.Services.AddControllers();

builder.Services.AddOutputCache(options =>
{
    options.AddPolicy("VideoStreamUrlCache", new StreamUrlResponseCachePolicy(["bvid", "avid", "page", "redirect"]));
    options.AddPolicy("LiveStreamUrlCache", new StreamUrlResponseCachePolicy(["roomId", "redirect"]));
});

builder.Host.UseSerilog();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/api-docs", () => Results.Content(
    $$"""
      <!doctype html>
      <html>
      <head>
          <title>MisakaBiliApi Reference</title>
          <meta charset="utf-8" />
          <meta name="viewport" content="width=device-width, initial-scale=1" />
      </head>
      <body>
          <script id="api-reference" data-url="/swagger/v1/swagger.json"></script>
          <script>
          var configuration = {}
      
          document.getElementById('api-reference').dataset.configuration =
              JSON.stringify(configuration)
          </script>
          <script src="https://cdn.jsdelivr.net/npm/@scalar/api-reference"></script>
      </body>
      </html>
      """,
    "text/html"
));

app.UseHttpsRedirection();

app.UseOutputCache();

app.MapControllers();

await app.RunAsync();
