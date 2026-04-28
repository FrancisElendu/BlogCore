using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text.Json;

namespace BlogCore.API.Middleware
{
    public static class GlobalExceptionHandler
    {
        public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                    if (contextFeature == null) return;

                    var exception = contextFeature.Error;
                    var traceId = Activity.Current?.Id ?? context.TraceIdentifier;
                    var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();

                    // Determine status code and response
                    var (statusCode, response) = CreateErrorResponse(exception, traceId);

                    // Log the exception
                    LogException(logger, exception, traceId, context.Request.Path);

                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "application/json";

                    var options = new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true
                    };

                    var jsonResponse = JsonSerializer.Serialize(response, options);
                    await context.Response.WriteAsync(jsonResponse);
                });
            });
        }

        private static (int StatusCode, BaseResponse<object> Response) CreateErrorResponse(Exception exception, string traceId)
        {
            var response = new BaseResponse<object>
            {
                Success = false,
                Errors = new Dictionary<string, string[]>()
            };

            switch (exception)
            {
                case ValidationException ex:
                    return (StatusCodes.Status400BadRequest, new BaseResponse<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Errors = ex.Errors
                    });

                case FluentValidation.ValidationException ex:
                    var validationErrors = ex.Errors
                        .GroupBy(e => e.PropertyName)
                        .ToDictionary(
                            g => g.Key,
                            g => g.Select(e => e.ErrorMessage).ToArray()
                        );
                    return (StatusCodes.Status400BadRequest, new BaseResponse<object>
                    {
                        Success = false,
                        Message = "Validation failed",
                        Errors = validationErrors
                    });

                case NotFoundException ex:
                    return (StatusCodes.Status404NotFound, new BaseResponse<object>
                    {
                        Success = false,
                        Message = ex.Message
                    });

                case UnauthorizedAccessException ex:
                    return (StatusCodes.Status401Unauthorized, new BaseResponse<object>
                    {
                        Success = false,
                        Message = ex.Message ?? "Unauthorized access"
                    });

                case ForbiddenAccessException ex:
                    return (StatusCodes.Status403Forbidden, new BaseResponse<object>
                    {
                        Success = false,
                        Message = ex.Message ?? "Access forbidden"
                    });

                case BusinessRuleException ex:
                    return (StatusCodes.Status400BadRequest, new BaseResponse<object>
                    {
                        Success = false,
                        Message = ex.Message
                    });

                case DuplicateException ex:
                    var errors = new Dictionary<string, string[]>();
                    if (ex.EntityName != null)
                    {
                        errors["entity"] = new[] { ex.EntityName };
                        if (ex.PropertyName != null) errors["property"] = new[] { ex.PropertyName };
                        if (ex.PropertyValue != null) errors["value"] = new[] { ex.PropertyValue };
                    }
                    return (StatusCodes.Status409Conflict, new BaseResponse<object>
                    {
                        Success = false,
                        Message = ex.Message,
                        Errors = errors.Any() ? errors : null
                    });

                case ConcurrencyException ex:
                    return (StatusCodes.Status409Conflict, new BaseResponse<object>
                    {
                        Success = false,
                        Message = ex.Message
                    });

                case ArgumentNullException ex:
                    return (StatusCodes.Status400BadRequest, new BaseResponse<object>
                    {
                        Success = false,
                        Message = $"Required parameter '{ex.ParamName}' was null"
                    });

                case ArgumentException ex:
                    return (StatusCodes.Status400BadRequest, new BaseResponse<object>
                    {
                        Success = false,
                        Message = ex.Message
                    });

                case InvalidOperationException ex:
                    return (StatusCodes.Status400BadRequest, new BaseResponse<object>
                    {
                        Success = false,
                        Message = ex.Message
                    });

                case DbUpdateConcurrencyException ex:
                    return (StatusCodes.Status409Conflict, new BaseResponse<object>
                    {
                        Success = false,
                        Message = "The data was modified by another user. Please refresh and try again."
                    });

                case TimeoutException ex:
                    return (StatusCodes.Status408RequestTimeout, new BaseResponse<object>
                    {
                        Success = false,
                        Message = "The request timed out. Please try again."
                    });

                default:
                    return HandleUnknownException(exception, traceId);
            }
        }

        private static (int StatusCode, BaseResponse<object> Response) HandleUnknownException(Exception exception, string traceId)
        {
#if DEBUG
            // Development: Show full error details
            return (StatusCodes.Status500InternalServerError, new BaseResponse<object>
            {
                Success = false,
                Message = exception.Message,
                Errors = new Dictionary<string, string[]>
                {
                    { "exceptionType", new[] { exception.GetType().Name } },
                    { "stackTrace", new[] { exception.StackTrace ?? string.Empty } },
                    { "traceId", new[] { traceId } }
                }
            });
#else
            // Production: Show generic message with trace ID for support
            return (StatusCodes.Status500InternalServerError, new BaseResponse<object>
            {
                Success = false,
                Message = "An error occurred while processing your request",
                Errors = new Dictionary<string, string[]>
                {
                    { "traceId", new[] { traceId } }
                }
            });
#endif
        }

        private static void LogException(ILogger logger, Exception exception, string traceId, string path)
        {
            switch (exception)
            {
                case ValidationException ex:
                    logger.LogWarning(ex, "Validation error on {Path}. TraceId: {TraceId}. Errors: {@Errors}",
                        path, traceId, ex.Errors);
                    break;

                case NotFoundException ex:
                    logger.LogInformation(ex, "Resource not found on {Path}. TraceId: {TraceId}",
                        path, traceId);
                    break;

                case UnauthorizedAccessException ex:
                    logger.LogWarning(ex, "Unauthorized access on {Path}. TraceId: {TraceId}",
                        path, traceId);
                    break;

                case ForbiddenAccessException ex:
                    logger.LogWarning(ex, "Forbidden access on {Path}. TraceId: {TraceId}",
                        path, traceId);
                    break;

                case BusinessRuleException ex:
                    logger.LogWarning(ex, "Business rule violation on {Path}. TraceId: {TraceId}",
                        path, traceId);
                    break;

                case DuplicateException ex:
                    logger.LogWarning(ex, "Duplicate entity on {Path}. TraceId: {TraceId}",
                        path, traceId);
                    break;

                case ConcurrencyException ex:
                    logger.LogWarning(ex, "Concurrency conflict on {Path}. TraceId: {TraceId}",
                        path, traceId);
                    break;

                case TimeoutException ex:
                    logger.LogError(ex, "Timeout on {Path}. TraceId: {TraceId}",
                        path, traceId);
                    break;

                default:
                    // Exception first parameter, then message template, then values
                    logger.LogError(exception, "Unhandled exception on {Path}. TraceId: {TraceId}",
                        path, traceId);
                    break;
            }
        }
    }

    //public static void UseGlobalExceptionHandler(this IApplicationBuilder app)
    //{
    //    app.UseExceptionHandler(appError =>
    //    {
    //        appError.Run(async context =>
    //        {
    //            context.Response.ContentType = "application/json";
    //            var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

    //            if (contextFeature != null)
    //            {
    //                var response = new BaseResponse<object>();

    //                switch (contextFeature.Error)
    //                {
    //                    case ValidationException ex:
    //                        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    //                        response.Success = false;
    //                        response.Message = ex.Message;
    //                        response.Errors = ex.Errors;
    //                        break;

    //                    case KeyNotFoundException ex:
    //                        context.Response.StatusCode = StatusCodes.Status404NotFound;
    //                        response.Success = false;
    //                        response.Message = ex.Message;
    //                        break;

    //                    case UnauthorizedAccessException ex:
    //                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
    //                        response.Success = false;
    //                        response.Message = ex.Message;
    //                        break;

    //                    default:
    //                        context.Response.StatusCode = StatusCodes.Status500InternalServerError;
    //                        response.Success = false;
    //                        response.Message = "An error occurred while processing your request";
    //                        break;
    //                }

    //                var jsonResponse = JsonSerializer.Serialize(response);
    //                await context.Response.WriteAsync(jsonResponse);
    //            }
    //        });
    //    });
    //}
}

