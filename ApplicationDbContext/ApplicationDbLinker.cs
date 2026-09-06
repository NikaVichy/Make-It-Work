using Microsoft.EntityFrameworkCore;

namespace MakeItWork.ApplicationDbContext
{
    public static class ApplicationDbLinker
    {
        public static IServiceCollection LinkDatabase(this IServiceCollection services, WebApplicationBuilder builder)
        {
            var connection = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(connection)) throw new InvalidOperationException("Environment variable DATABASE_CONNECTION_STRING is missing");

            services.AddDbContext<ApplicationDb>(options => options.UseNpgsql(connection));

            return services;
        }
    }
}
