using BlogCore.Application.Common.Base;
using BlogCore.Application.Common.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
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
                    context.Response.ContentType = "application/json";
                    var contextFeature = context.Features.Get<IExceptionHandlerFeature>();

                    if (contextFeature != null)
                    {
                        var response = new BaseResponse<object>();
                        var exception = contextFeature.Error;

                        switch (exception)
                        {
                            case ValidationException ex:
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                response.Success = false;
                                response.Message = ex.Message;
                                response.Errors = ex.Errors;
                                break;

                            case FluentValidation.ValidationException ex:
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                response.Success = false;
                                response.Message = "Validation failed";
                                response.Errors = ex.Errors
                                    .GroupBy(e => e.PropertyName)
                                    .ToDictionary(
                                        g => g.Key,
                                        g => g.Select(e => e.ErrorMessage).ToArray()
                                    );
                                break;

                            case NotFoundException ex:
                                context.Response.StatusCode = StatusCodes.Status404NotFound;
                                response.Success = false;
                                response.Message = ex.Message;
                                break;

                            case UnauthorizedAccessException ex:
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                response.Success = false;
                                response.Message = ex.Message;
                                break;

                            case BusinessRuleException ex:
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                response.Success = false;
                                response.Message = ex.Message;
                                break;

                            case ForbiddenAccessException ex:
                                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                                response.Success = false;
                                response.Message = ex.Message;
                                break;

                            case DuplicateException ex:
                                context.Response.StatusCode = StatusCodes.Status409Conflict;
                                response.Success = false;
                                response.Message = ex.Message;
                                if (ex.EntityName != null)
                                {
                                    response.Errors = new Dictionary<string, string[]>
                                    {
                                        { "entity", new[] { ex.EntityName } },
                                        { "property", new[] { ex.PropertyName ?? "unknown" } },
                                        { "value", new[] { ex.PropertyValue ?? "duplicate" } }
                                    };
                                }
                                break;

                            case ConcurrencyException ex:
                                context.Response.StatusCode = StatusCodes.Status409Conflict;
                                response.Success = false;
                                response.Message = ex.Message;
                                break;

                            default:
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                                response.Success = false;

                                // Different behavior based on build configuration
#if DEBUG
                                // During development: Show full error details
                                response.Message = exception.Message;
                                response.Errors = new Dictionary<string, string[]>
                                {
                                    { "exceptionType", new[] { exception.GetType().Name } },
                                    { "message", new[] { exception.Message } },
                                    { "stackTrace", new[] { exception.StackTrace ?? string.Empty } }
                                };
#else
                                // In production: Show generic message only
                                response.Message = "An error occurred while processing your request";
#endif
                                break;
                        }

                        var options = new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            WriteIndented = true
                        };

                        var jsonResponse = JsonSerializer.Serialize(response, options);
                        await context.Response.WriteAsync(jsonResponse);
                    }
                });
            });
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

