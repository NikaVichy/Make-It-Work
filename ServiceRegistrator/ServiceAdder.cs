using MakeItWork.Models;
using MakeItWork.ServiceOptions;
using MakeItWork.Services.ModelServices.BaseRepository;
using MakeItWork.Services.ModelServices.ProjectRepository;
using MakeItWork.Services.ModelServices.ProjectService;
using MakeItWork.Services.ModelServices.TaskRepository;
using MakeItWork.Services.ModelServices.TaskService;
using MakeItWork.Services.ModelServices.UnitOfWork;
using MakeItWork.Services.ModelServices.UserAuthRepository;
using MakeItWork.Services.ModelServices.UserRepository;
using MakeItWork.Services.SecurityServices.Authenticator;
using MakeItWork.Services.SecurityServices.Authorizer;
using MakeItWork.Services.SecurityServices.PasswordHasher;
using MakeItWork.Services.SecurityServices.PrincipleValidator;
using MakeItWork.Services.UserRegistration;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace MakeItWork.ServiceRegistrator
{
    public static class ServiceRegistrationExtension
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Security services
            var key = Environment.GetEnvironmentVariable("TOTP_ENCRYPTION_MASTER");
            if (string.IsNullOrWhiteSpace(key))
                throw new InvalidOperationException("Environment variable TOTP_ENCRYPTION_MASTER is missing");

            services.AddScoped<IAuthorizerService, AuthorizerService>();


            services.Configure<PasswordHasherOptions>(configuration.GetSection(nameof(PasswordHasherOptions)));
            services.AddTransient<IPasswordHasherService, PasswordHasherService>();

            // Misc services

            services.AddRazorPages(options => { });


            services.AddScoped<IUnitOfWork, EfUnitOfWork>();

            services.AddScoped<IProjectRepository, ProjectRepository>();
            services.AddScoped<ITaskRepository, TaskRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserAuthRepository, UserAuthRepository>();

            services.AddScoped<IProjectService, ProjectService>();
            services.AddScoped<ITaskService, TaskService>();


            services.AddScoped<IUserRegistrationService, UserRegistrationService>();
            services.AddScoped<IAuthenticatorService, AuthenticatorService>();

            return services;
        }

        public static IServiceCollection AddConfiguredAuthentication(this IServiceCollection services, CookieSecurePolicy securePolicy)
        {
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/login";
                    options.LogoutPath = "/logout";
                    options.Cookie.HttpOnly = true;
                    options.Cookie.SecurePolicy = securePolicy;
                    options.Cookie.SameSite = SameSiteMode.Strict;
                    options.ExpireTimeSpan = TimeSpan.FromDays(7);
                    options.SlidingExpiration = true;

                    options.Events.OnValidatePrincipal = PrincipleValidation.ValidatePrincipal;
                });

            return services;
        }
        public static IServiceCollection AddConfiguredAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization();
            return services;
        }

        public static IServiceCollection AddConfiguredRateLimiter(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.OnRejected = async (context, ct) =>
                {
                    context.HttpContext.Response.Headers["Retry-After"] = "60";

                    await context.HttpContext.Response.WriteAsync(
                        "Too many requests. Try again later.", ct);
                };

                options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
                {
                    if (context.User.Identity?.IsAuthenticated == true)
                    {
                        var key = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "null";
                        return RateLimitPartition.GetTokenBucketLimiter(
                            partitionKey: $"token:{key}",
                            factory: _ => new TokenBucketRateLimiterOptions
                            {
                                TokenLimit = 50,
                                TokensPerPeriod = 2,
                                ReplenishmentPeriod = TimeSpan.FromSeconds(1),
                                AutoReplenishment = true,
                                QueueLimit = 0
                            });
                    }
                    else
                    {
                        var key = context.Connection.RemoteIpAddress?.ToString() ?? "null";

                        return RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey: $"ip:{key}",
                            factory: _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = 5,
                                Window = TimeSpan.FromMinutes(1),
                                AutoReplenishment = true
                            });
                    }
                });

                options.AddPolicy<string>("auth", context =>
                {
                    var ip = context.Connection.RemoteIpAddress?.ToString() ?? "null";

                    return RateLimitPartition.GetFixedWindowLimiter(
                        partitionKey: $"auth:{ip}",
                        factory: _ => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 5,
                            Window = TimeSpan.FromMinutes(5),
                            AutoReplenishment = true,
                            QueueLimit = 0
                        });
                });
            });

            return services;
        }
    }
}
