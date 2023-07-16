using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using MisakaBiliApi.Models.ApiResponse;

namespace MisakaBiliApi.Filters;

public class ApiActionFilter : IActionFilter, IOrderedFilter
{
    private readonly ILogger<ApiActionFilter> _logger;

    public ApiActionFilter(ILogger<ApiActionFilter> logger)
    {
        _logger = logger;
    }

    public int Order => int.MaxValue - 10;

    public void OnActionExecuting(ActionExecutingContext context)
    {
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Exception is { } exception)
        {
            _logger.LogError(context.Exception, "Exception when {Method} {Url}", context.HttpContext.Request.Method,
                context.HttpContext.Request.GetEncodedPathAndQuery());

            context.Result =
                new BadRequestObjectResult(new MisakaApiResponse(exception.Message,
                    StatusCodes.Status500InternalServerError))
                {
                    StatusCode = StatusCodes.Status500InternalServerError
                };

            context.ExceptionHandled = true;
        }

        if (context.Result is not IStatusCodeActionResult result) return;

        var statusCode = result.StatusCode ?? StatusCodes.Status200OK;

        if (context.Result is ObjectResult objectResult)
        {
            if (objectResult.Value is not string) return;

            context.Result =
                new ObjectResult(new MisakaApiResponse(objectResult.Value.ToString() ?? string.Empty, statusCode))
                {
                    StatusCode = statusCode
                };

            return;
        }

        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(error => error.Value != null && error.Value.Errors.Count > 0)
                .Select(error => error.Value?.Errors.First().ErrorMessage)
                .ToArray();

            var errorMessage = string.Join(", ", errors);

            context.Result =
                new BadRequestObjectResult(new MisakaApiResponse(errorMessage, StatusCodes.Status400BadRequest));
            
            return;
        }

        context.Result = new ObjectResult(new MisakaApiResponse("", statusCode))
        {
            StatusCode = statusCode
        };
    }
}