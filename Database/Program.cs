using Microsoft.EntityFrameworkCore;
using Serilog;
using Database.Data;

namespace Database;

public class Program
{
    public static void Main(string[] args)
    {
        ConfigureLogging();

        try
        {
            Log.Information("Starting Database service...");

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddSerilog();

            ConfigureDatabase(builder);

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            Log.Information("Database service configured successfully");
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Database service failed to start");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/database-service.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    private static void ConfigureDatabase(WebApplicationBuilder builder)
    {
        Log.Information("Attempting to connect to the database");   
        
        var dbDefaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                                        throw new InvalidOperationException("Missing environment variable DefaultConnection");

        builder.Services.AddDbContext<MyDbContext>(options => { options.UseNpgsql(dbDefaultConnectionString); });

        Log.Information("Database connection established");
    }
}