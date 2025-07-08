using AuthECAPI.Models;
using Microsoft.AspNetCore.Builder;

namespace AuthECAPI.Extensions;

public static class AppConfigExtensions
{
    public static WebApplication ConfigureCORS(this WebApplication app, IConfiguration configuration)
    {
        app.UseCors();
        return app;
    }
    public static IServiceCollection AddAppConfig(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<AppSettings>(configuration.GetSection("AppSettings"));
        return services;
    }
    public static WebApplication ConfigureAppConfig(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        return app;
    }
}
