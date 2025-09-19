using Auth.Services;
using Serilog;
using Serilog.Filters;

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

            // app.UseSerilogRequestLogging();
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

    private static void ConfigureLogging()
    {   
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: "logs/all/auth-service-all.log",
                rollingInterval: RollingInterval.Day,
                shared: true)
            .WriteTo.Logger(lc => lc
                .Enrich.FromLogContext()
                .Filter.ByExcluding(Matching.FromSource("System"))
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .Filter.ByExcluding(Matching.FromSource("Serilog.AspNetCore"))
                .WriteTo.File(
                    path: "logs/auth-service.log",
                    rollingInterval: RollingInterval.Day,
                    shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"))
            .CreateLogger();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSerilog();
        services.AddHttpClient();

        var databaseServiceUrl = Environment.GetEnvironmentVariable("DATABASE_SERVICE_URL") ??
                                 throw new InvalidOperationException("Missing environment variable DATABASE_SERVICE_URL");

        services.AddHttpClient<DatabaseService>(client => { client.BaseAddress = new Uri(databaseServiceUrl); });

        services.AddScoped<SubscriptionService>();
        services.AddScoped<PaymentService>();

        services.AddControllers();
    }
}
