using Serilog;

namespace Auth;

public class Program
{
    public static void Main(string[] args)
    {
        ConfigureLogging();

        try
        {
            Log.Information("Starting Auth service...");
            
            var builder = WebApplication.CreateBuilder(args);
            
            ConfigureServices(builder.Services);

            var app = builder.Build();

            app.UseSerilogRequestLogging();
            app.MapControllers();

            Log.Information("Auth service configured successfully");
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
            throw;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSerilog();
        services.AddHttpClient();
        services.AddControllers();
    }

    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .WriteTo.File(
                path: "logs/auth-service.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }
}