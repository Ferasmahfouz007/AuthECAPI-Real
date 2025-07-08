using AuthECAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AuthECAPI.Extensions;

public static class IdentityExtensions
{
    public static IServiceCollection AddIdentityHandlersAndStores(this IServiceCollection services)
    {
        services.AddIdentityApiEndpoints<AppUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<AppDbContext>();
        return services;

    }
    public static IServiceCollection ConfigureIdentityOptions(this IServiceCollection services)
    {
        services.Configure<IdentityOptions>(options =>
        {
            // Password settings
            options.Password.RequireDigit = false;
            options.Password.RequiredLength = 6;
            options.Password.RequireLowercase = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            // User settings
            options.User.RequireUniqueEmail = true;
        });
        return services;
    }

    //Auth = Authentication + Autherization
    public static IServiceCollection AddIdentityAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
                {
                    options.SaveToken = false;
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = ctx =>
                        {
                            Console.WriteLine("JWT FAILED: " + ctx.Exception.Message);
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = ctx =>
                        {
                            Console.WriteLine("Headers: " + string.Join(", ", ctx.Request.Headers.Select(h => $"{h.Key}: {h.Value}")));
                            Console.WriteLine("Received Token: " + ctx.Token);
                            return Task.CompletedTask;
                        }
                    };
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(
                                Encoding.UTF8.GetBytes(configuration["AppSettings:JWTSecret"]!)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                    };
                });
        // To allow anonymous access to Swagger endpoints, add a policy exception for Swagger in AddAuthorization
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                .RequireAuthenticatedUser()
                .Build();            
        });
        return services;
    }
    public static WebApplication AddIdentityAuthMiddlewares(this WebApplication app)
    {
        // Configure the HTTP request pipeline.
        app.UseAuthentication();
        app.UseAuthorization();
        return app;
    }
}




////services refers to the IServiceCollection object that is used to register services in the application's Dependency Injection (DI) container.
//services.AddAuthorization(//AddAuthorization is a method that configures the authorization services for the application.
//    options =>//options is an object of type AuthorizationOptions, which allows you to configure various settings related to authorization policies.
//            {
//                //The FallbackPolicy is a special policy that will be used as the default authorization policy whenever no other policy explicitly applies.
//                options.FallbackPolicy = new AuthorizationPolicyBuilder()
//            .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme) //This line specifies which authentication schemes the policy should support.
//            .RequireAuthenticatedUser()
//            .Build();
