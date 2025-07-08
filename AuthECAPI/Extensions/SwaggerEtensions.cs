using Microsoft.AspNetCore.Routing.Matching;
using Microsoft.OpenApi.Models;

namespace AuthECAPI.Extensions;

public static class SwaggerEtensions
{
    public static IServiceCollection AddSwaggerExplorer(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options=>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Fill in the JWT token",
            });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
        });
        return services;
    }
    public static WebApplication ConfigureSwaggerExplorer(this WebApplication app)
    {       
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            //app.MapOpenApi();
        }
        return app;
    }
}
