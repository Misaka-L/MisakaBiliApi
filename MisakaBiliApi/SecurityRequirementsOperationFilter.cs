using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace MisakaBiliApi;

public class SecurityRequirementsOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attributes = new List<AuthorizeAttribute>();

        attributes.AddRange(context.MethodInfo.GetCustomAttributes(true).OfType<AuthorizeAttribute>());
        attributes.AddRange(context.MethodInfo.DeclaringType?.GetCustomAttributes(true).OfType<AuthorizeAttribute>() ?? []);

        var requiredScopes = attributes
            .Select(attr => attr.Policy)
            .Distinct()
            .ToArray();

        if (requiredScopes.Length == 0) return;

        operation.Responses.Add("401", new OpenApiResponse { Description = "Unauthorized" });

        var authScheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "ApiKey" }
        };

        operation.Security = new List<OpenApiSecurityRequirement>
        {
            new()
            {
                [ authScheme ] = requiredScopes.ToList()
            }
        };
    }
}
