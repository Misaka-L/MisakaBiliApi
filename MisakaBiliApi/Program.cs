using System.Diagnostics;
using System.Net;
using System.Net.Mime;
using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Interfaces;
using Microsoft.OpenApi.Models;
using Microsoft.OpenApi.Services;
using MisakaBiliApi;
using MisakaBiliApi.Filters;
using MisakaBiliApi.Forwarder;
using MisakaBiliApi.Models.ApiResponse;
using MisakaBiliApi.Services;
using Refit;
using Serilog;
using Serilog.Templates;
using Serilog.Templates.Themes;
using Yarp.ReverseProxy.Forwarder;

var logTemplate =
    "[{@t:yyyy-MM-dd HH:mm:ss} {@l:u3}] [{Substring(SourceContext, LastIndexOf(SourceContext, '.') + 1)}] {@m}\n{@x}";

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .WriteTo.File(new ExpressionTemplate(logTemplate), "logs/app-.log", rollingInterval: RollingInterval.Day)
    .WriteTo.Console(new ExpressionTemplate(logTemplate, theme: TemplateTheme.Literate))
    .WriteTo.Debug(new ExpressionTemplate(logTemplate))
    .CreateLogger();

// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.Add<ExceptionFilter>();
    options.Filters.Add<ApiActionFilter>();
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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

builder.Services.AddRefitClient<IBiliApiServer>()
    .ConfigureHttpClient(client => client.BaseAddress = new Uri("https://api.bilibili.com"));

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