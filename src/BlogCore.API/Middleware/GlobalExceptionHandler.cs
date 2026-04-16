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

                        switch (contextFeature.Error)
                        {
                            case ValidationException ex:
                                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                                response.Success = false;
                                response.Message = ex.Message;
                                response.Errors = ex.Errors;
                                break;

                            case KeyNotFoundException ex:
                                context.Response.StatusCode = StatusCodes.Status404NotFound;
                                response.Success = false;
                                response.Message = ex.Message;
                                break;

                            case UnauthorizedAccessException ex:
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                                response.Success = false;
                                response.Message = ex.Message;
                                break;

                            default:
                                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                                response.Success = false;
                                response.Message = "An error occurred while processing your request";
                                break;
                        }

                        var jsonResponse = JsonSerializer.Serialize(response);
                        await context.Response.WriteAsync(jsonResponse);
                    }
                });
            });
        }
    }
}
