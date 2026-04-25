using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Back_EndAPI.Services;
using System.Text.Json;

namespace Back_EndAPI.Filters;

/// <summary>
/// Action filter to handle idempotent operations
/// Checks for Idempotency-Key header and prevents duplicate processing
/// </summary>
[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
public class IdempotentAttribute : Attribute, IAsyncActionFilter
{
    private readonly string _operationName;

    public IdempotentAttribute(string operationName)
    {
        _operationName = operationName;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Get idempotency key from header
        var idempotencyKey = context.HttpContext.Request.Headers["Idempotency-Key"].FirstOrDefault();

        // If no idempotency key provided, proceed normally
        if (string.IsNullOrWhiteSpace(idempotencyKey))
        {
            await next();
            return;
        }

        // Get the idempotency service
        var idempotencyService = context.HttpContext.RequestServices.GetService<IdempotencyService>();
        if (idempotencyService == null)
        {
            await next();
            return;
        }

        // Check if this request has already been processed
        var existingRecord = await idempotencyService.GetExistingRecordAsync(idempotencyKey, _operationName);

        if (existingRecord != null)
        {
            // Return the cached response
            context.Result = new ObjectResult(JsonSerializer.Deserialize<object>(existingRecord.ResponseBody))
            {
                StatusCode = existingRecord.StatusCode
            };
            return;
        }

        // Proceed with the action
        var executedContext = await next();

        // Store the result if successful
        if (executedContext.Result is ObjectResult objectResult && objectResult.Value != null)
        {
            var statusCode = objectResult.StatusCode ?? 200;

            // Only cache successful responses (2xx)
            if (statusCode >= 200 && statusCode < 300)
            {
                var userId = context.HttpContext.User?.Identity?.Name;
                await idempotencyService.RecordOperationAsync(
                    idempotencyKey,
                    _operationName,
                    statusCode,
                    objectResult.Value,
                    userId: userId
                );
            }
        }
    }
}
