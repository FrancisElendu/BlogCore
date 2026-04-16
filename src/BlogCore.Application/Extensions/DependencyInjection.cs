using BlogCore.Application.Common.Behaviors;
using FluentValidation;
using MediatR;
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
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>))
                .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        }
    }

}
