using MakeItWork.ServiceOptions;
using MakeItWork.Services.ModelControllers.ProjectController;
using MakeItWork.Services.ModelControllers.TaskControllerService;
using MakeItWork.Services.ModelControllers.UserAuthController;
using MakeItWork.Services.ModelControllers.UserController;
using MakeItWork.Services.SecurityServices.Authenticator;
using MakeItWork.Services.SecurityServices.Authorizer;
using MakeItWork.Services.SecurityServices.PasswordHasher;
using MakeItWork.Services.SecurityServices.PrincipleValidator;
using MakeItWork.Services.UserRegistration;
using Microsoft.AspNetCore.Authentication.Cookies;

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

            services.AddScoped<IProjectController, ProjectControllerService>();
            services.AddScoped<ITaskController, TaskControllerService>();
            services.AddScoped<IUserController, UserControllerService>();
            services.AddScoped<IUserAuthController, UserAuthControllerService>();

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
    }
}
