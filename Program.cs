using MakeItWork.ApplicationDbContext;
using MakeItWork.ServiceRegistrator;
using dotenv.net;
using MakeItWork.Services.SecurityServices;

namespace MakeItWork
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            DotEnv.Load();

            if (!builder.Environment.IsDevelopment()) 
                SecurityConfigurator.AddConfiguredSecurity(builder);

            builder.Services.LinkDatabase(builder);

            var securePolicy = builder.Environment.IsProduction() ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
            builder.Services.AddConfiguredAuthentication(securePolicy);
            builder.Services.AddConfiguredAuthorization();
            builder.Services.AddConfiguredRateLimiter();

            builder.Services.AddCustomServices(builder.Configuration);

            if (builder.Environment.IsDevelopment())
            {
                builder.Services.AddDistributedMemoryCache();
                builder.Services.AddSession();
            }

            var app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();

            if (!app.Environment.IsDevelopment())
            {
                throw new("Add domain into https://hstspreload.org/");
                app.UseExceptionHandler("/ServerError");
            }
            else
                app.UseDeveloperExceptionPage();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseRateLimiter();

            SecurityConfigurator.UseConfiguredSecurity(app);

            app.MapRazorPages();

            app.Run();
        }
    }
}