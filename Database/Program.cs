using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;
using Database.Data;
using Database.Services;

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

            ConfigureServices(builder);

            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.MapControllers();

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
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: "logs/all/database-service-all.log",
                rollingInterval: RollingInterval.Day,
                shared: true)
            .WriteTo.Logger(lc => lc
                .Enrich.FromLogContext()
                .Filter.ByExcluding(Matching.FromSource("System"))
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .Filter.ByExcluding(Matching.FromSource("Serilog.AspNetCore"))
                .WriteTo.File(
                    path: "logs/database-service.log",
                    rollingInterval: RollingInterval.Day,
                    shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"))
            .CreateLogger();
    }

    private static void ConfigureServices(WebApplicationBuilder builder)
    {
        builder.Services.AddSerilog();
        
        var dbDefaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                                        throw new InvalidOperationException("Missing environment variable DefaultConnection");

        builder.Services.AddDbContext<MyDbContext>(options => { options.UseNpgsql(dbDefaultConnectionString); });

        builder.Services.AddScoped<TranscriptionService>();
        
        builder.Services.AddControllers();
    }
}