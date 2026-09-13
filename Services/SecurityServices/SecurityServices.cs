namespace MakeItWork.Services.SecurityServices
{
    public static class SecurityConfigurator
    {
        public static void AddConfiguredSecurity(WebApplicationBuilder builder)
        {
            AddConfiguredHSTS(builder);
            AddConfiguredHTTPSRedirect(builder);
        }
        public static void AddConfiguredHSTS(WebApplicationBuilder builder)
        {
            builder.Services.AddHsts(options =>
            {
                options.Preload = false;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(365);
                options.ExcludedHosts.Add("localhost");
                options.ExcludedHosts.Add("127.0.0.1");
            });
        }
        public static void AddConfiguredHTTPSRedirect(WebApplicationBuilder builder, ushort HTTPSPort = 443)
        {
            builder.Services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = HTTPSPort;
                options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
            });
        }

        public static void UseConfiguredSecurity(WebApplication app)
        {
            app.UseHsts();
            app.UseHttpsRedirection();

            app.Use(async (context, next) =>
            {
                context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
                context.Response.Headers.Append("X-Frame-Options", "DENY");
                context.Response.Headers.Append("X-XSS-Protextion", "1; mode=block");
                context.Response.Headers.Append("Referrer-Policy", "strict-origin-when-cross-origin");
                context.Response.Headers.Append("Permissions-Policy", "camera=(), microphone=(), geolocation=()");

                await next();
            });
        }
    }
}
