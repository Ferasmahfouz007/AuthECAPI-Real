using AuthECAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace AuthECAPI.Extensions;

public static class EFCoreExtensions
{
    public static IServiceCollection InjectDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        // Bind configuration settings into DbSettings object
        services.Configure<DbSettings>(configuration.GetSection("DbSettings"));
        services.AddDbContext<AppDbContext>((serviceProvider,options) =>
        {
            var dbSettings = serviceProvider.GetRequiredService<IOptions<DbSettings>>().Value;
            options.UseSqlServer(dbSettings.DevDbConnectionString);
        });
        return services;
    }
}
