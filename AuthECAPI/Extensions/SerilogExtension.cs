using AuthECAPI.Models;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using System.Configuration;

namespace AuthECAPI.Extensions;

public static class SerilogExtension
{
    public static void AddSerilog(IConfiguration configuration)
    {
        //Log.Logger = new LoggerConfiguration()
        //    .MinimumLevel.Debug()
        //    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
        //    .Enrich.FromLogContext()
        //    .WriteTo.Console()
        //    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
        //    .CreateLogger();
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();
    }
    public static void AddSerlogForTable(IConfiguration configuration)
    {
        var dbSettings = configuration.GetSection("DbSettings").Get<DbSettings>();
        var connectionString = dbSettings.DevDbConnectionString;
        // Optional: additional column configuration
        var columnOptions = new ColumnOptions();
        columnOptions.Store.Remove(StandardColumn.Properties);
        columnOptions.Store.Add(StandardColumn.LogEvent);
        Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.MSSqlServer(
        connectionString: connectionString,
        sinkOptions: new MSSqlServerSinkOptions
        {
            TableName = "Logs",
            AutoCreateSqlTable = true // Set false if you already created it manually
        },
        columnOptions: columnOptions
    )
    .CreateLogger();
    }
}
