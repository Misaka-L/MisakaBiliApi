using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MisakaBiliApi.Models.ApiResponse;

namespace MisakaBiliApi.Filters;

public class ExceptionFilter : IExceptionFilter, IOrderedFilter
{
    private ILogger<ExceptionFilter> _logger;

    public ExceptionFilter(ILogger<ExceptionFilter> logger)
    {
        _logger = logger;
    }

    public int Order => int.MaxValue - 10;

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "Exception when {Method} {Url}", context.HttpContext.Request.Method,
            context.HttpContext.Request.GetEncodedPathAndQuery());

        context.Result =
            new BadRequestObjectResult(new MisakaApiResponse(context.Exception.Message,
                StatusCodes.Status500InternalServerError))
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };

        context.ExceptionHandled = true;
    }
}