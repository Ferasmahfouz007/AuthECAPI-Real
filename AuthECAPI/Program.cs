using AuthECAPI.Controllers;
using AuthECAPI.Extensions;
using AuthECAPI.Models;
using Serilog;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
//builder.Services.AddOpenApi();


builder.Services.AddSwaggerExplorer()
                .InjectDbContext(builder.Configuration)
                .AddAppConfig(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration);

SerilogExtension.AddSerilog(builder.Configuration);
SerilogExtension.AddSerlogForTable(builder.Configuration);
builder.Host.UseSerilog();

var app = builder.Build();

//app.UseHttpsRedirection();
app.ConfigureSwaggerExplorer()
   .ConfigureCORS(builder.Configuration)
   .AddIdentityAuthMiddlewares();

app.MapControllers();
app.MapGroup("/api")
    .MapIdentityApi<AppUser>();
 app.MapGroup("/api")
    .MapIdentityUserEndpoints()
    .MapAccountEndpoints();


app.Run();
