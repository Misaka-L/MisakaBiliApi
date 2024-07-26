using System.Diagnostics;
using System.Net;
using System.Reflection;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using MisakaBiliApi.Forwarder;
using MisakaBiliCore;
using MisakaBiliCore.Services;
using MisakaBiliCore.Services.BiliApi;
using Refit;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;
using Yarp.ReverseProxy.Forwarder;

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

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Version = "v1",
        Title = "Misaka-L's Bili Api",
        Description = "A simple api.",
        Contact = new OpenApiContact()
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

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));
builder.Services.AddHttpForwarder();

builder.Services.AddRefitClient<IBiliApiServices>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://api.bilibili.com"));
builder.Services.AddTransient<WbiRequestHandler>();
builder.Services.AddHttpClient("biliapi", client =>
{
    foreach (var (headerName, value) in defaultRequestHeader)
    {
        client.DefaultRequestHeaders.Add(headerName, value);
    }

    client.BaseAddress = new Uri("https://api.bilibili.com");
}).ConfigurePrimaryHttpMessageHandler(() => new SocketsHttpHandler
{
    AutomaticDecompression = DecompressionMethods.All,
    CookieContainer = cookieContainer
});
builder.Services.AddRefitClient<IBiliApiServices>(null, httpClientName: "biliapi")
    .AddHttpMessageHandler<WbiRequestHandler>();

builder.Services.AddControllers();

builder.Host.UseSerilog();

var app = builder.Build();

app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

Configure(app);

app.MapControllers();

app.MapReverseProxy();

app.Run();

void Configure(IApplicationBuilder app)
{
    var httpClient = new HttpMessageInvoker(new SocketsHttpHandler()
    {
        UseProxy = false,
        AllowAutoRedirect = false,
        AutomaticDecompression = DecompressionMethods.None,
        UseCookies = false,
        ActivityHeadersPropagator = new ReverseProxyPropagator(DistributedContextPropagator.Current),
        ConnectTimeout = TimeSpan.FromSeconds(15),
    });

    var transformer = new BiliVideoTransformer();
    var requestConfig = new ForwarderRequestConfig { ActivityTimeout = TimeSpan.FromSeconds(100) };

    app.UseRouting();
    app.UseAuthorization();
    app.UseEndpoints(endpoints =>
    {
        endpoints.Map("/forward/bilibili/{**catch-all}", async httpContext =>
        {
            var proxyHost = new Uri("https://" + httpContext.Request.Path.Value?.Replace("/forward/bilibili/", "") ?? string.Empty)
                .Host;
            if (!(httpContext.Request.Path.HasValue & (proxyHost.EndsWith("bilivideo.com") || proxyHost.EndsWith("akamaized.net"))))
            {
                httpContext.Response.StatusCode = 400;
                return;
            }

            var error = await app.ApplicationServices.GetRequiredService<IHttpForwarder>().SendAsync(httpContext,
                "https://" + proxyHost,
                httpClient, requestConfig, transformer);
            // Check if the operation was successful
            if (error != ForwarderError.None)
            {
                var errorFeature = httpContext.GetForwarderErrorFeature();
                var exception = errorFeature.Exception;
            }
        });
    });
}
