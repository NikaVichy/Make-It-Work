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
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(730);
                options.ExcludedHosts.Clear();
            });
        }
        public static void AddConfiguredHTTPSRedirect(WebApplicationBuilder builder, ushort HTTPSPort = 443)
        {
            builder.Services.AddHttpsRedirection(options =>
            {
                options.HttpsPort = HTTPSPort;
                options.RedirectStatusCode = StatusCodes.Status301MovedPermanently;
            });
        }

        public static void UseConfiguredSecurity(WebApplication app)
        {
            app.UseHsts();
            app.UseHttpsRedirection();
        }
    }
}
