using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace BlogCore.Application.Extensions
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Register application services here
            var assembly = Assembly.GetExecutingAssembly();
            return services
                .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembly))
                .AddValidatorsFromAssembly(assembly)
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationPipelineBehavior<,>));
        }
    }


    //public static class DependencyInjection
    //{
    //    public static void AddApplicatione(this IServiceCollection services, IConfiguration config)
    //    {
    //        // Add MediatR with handlers from Application layer
    //        services.AddMediatR(cfg =>
    //        {
    //            cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(BlogCore.Application.DTOs.Auth.UserManagementResponseDto)));
    //        });

    //        // Add FluentValidation
    //        services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(BlogCore.Application.Features.Admin.Commands.AddClaimToUser.AddClaimToUserCommandValidator)));

    //        // Add Pipeline Behaviors
    //        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
    //        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
    //    }
    //}
}
