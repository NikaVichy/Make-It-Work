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

            if (!builder.Environment.IsDevelopment()) SecurityConfigurator.AddConfiguredSecurity(builder);
            builder.Services.LinkDatabase(builder);

            var securePolicy = builder.Environment.IsProduction() ? CookieSecurePolicy.Always : CookieSecurePolicy.SameAsRequest;
            builder.Services.AddConfiguredAuthentication(securePolicy);
            builder.Services.AddConfiguredAuthorization();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            builder.Services.AddCustomServices(builder.Configuration);

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                throw new("Add domain into https://hstspreload.org/");
                SecurityConfigurator.UseConfiguredSecurity(app);

                app.UseExceptionHandler("/ServerError");
            }
            else
            {
                app.UseExceptionHandler("/ServerError");

                //app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();

            app.MapRazorPages();

            app.Run();
        }
    }
}