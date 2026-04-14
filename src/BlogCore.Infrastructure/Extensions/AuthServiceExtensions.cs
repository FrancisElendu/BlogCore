using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace BlogCore.Infrastructure.Extensions
{
    public static class AuthServiceExtensions
    {
        /// <summary>
        /// Adds JWT Authentication to the service collection
        /// </summary>
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secret = jwtSettings["Secret"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
                    ClockSkew = TimeSpan.Zero // Remove delay when token expires
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;

                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/signalr"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }

        /// <summary>
        /// Adds Authorization Policies to the service collection
        /// </summary>
        public static IServiceCollection AddAuthorizationPolicies(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                // Role-based policies
                options.AddPolicy("AdminOnly", policy =>
                    policy.RequireRole("Admin"));

                options.AddPolicy("AuthorOrAdmin", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Admin") ||
                        context.User.IsInRole("Author")));

                options.AddPolicy("EditorOrAdmin", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Admin") ||
                        context.User.IsInRole("Editor")));

                // Claim-based policies
                options.AddPolicy("CanEditPosts", policy =>
                    policy.RequireClaim("Permission", "posts.edit.own", "posts.edit.all"));

                options.AddPolicy("CanDeletePosts", policy =>
                    policy.RequireClaim("Permission", "posts.delete.all"));

                options.AddPolicy("CanViewAllPosts", policy =>
                    policy.RequireAssertion(context =>
                        context.User.HasClaim("Permission", "posts.view.all") ||
                        context.User.IsInRole("Admin")));

                options.AddPolicy("CanCreatePosts", policy =>
                    policy.RequireClaim("Permission", "posts.create"));

                options.AddPolicy("CanManageUsers", policy =>
                    policy.RequireClaim("Permission", "users.manage"));

                // Combination policies
                options.AddPolicy("ContentModerator", policy =>
                    policy.RequireAssertion(context =>
                        context.User.IsInRole("Admin") ||
                        context.User.IsInRole("Editor") ||
                        context.User.HasClaim("Permission", "posts.edit.all")));
            });

            return services;
        }

        /// <summary>
        /// Adds both JWT Authentication and Authorization Policies
        /// </summary>
        public static IServiceCollection AddJwtAuthenticationAndPolicies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddJwtAuthentication(configuration);
            services.AddAuthorizationPolicies();

            return services;
        }
    }
}
