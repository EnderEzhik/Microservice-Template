using Microsoft.AspNetCore.Mvc;
using Serilog;
using Serilog.Filters;
using TranscriptionsWorker.Services;

namespace TranscriptionsWorker;

public class Program
{
    public static void Main(string[] args)
    {
        ConfigureLogging();

        try
        {
            Log.Information("TranscriptionsWorker starting...");
            var builder = WebApplication.CreateBuilder(args);
            
            ConfigureServices(builder.Services);

            var app = builder.Build();

            app.UseSerilogRequestLogging();

            app.MapGet("transcriptions", async (TranscriptionService transcriptionService, [FromQuery] string url) =>
            {
                Log.Information("GET request for get transcription for url: {Url}", url);

                var transcription = await transcriptionService.FindTranscription(url);
                return transcription != null ? Results.Ok(transcription) : Results.NotFound();
            });

            app.MapPost("transcriptions", async (TranscriptionService transcriptionService, CreateTranscriptionRequest request) =>
            {
                var url = request.Url;
                Log.Information("POST request for create transcription for url: {Url}", url);

                try
                {
                    var transcription = await transcriptionService.CreateTranscription(url);
                    return Results.Ok(transcription);
                }
                catch
                {
                    return Results.Problem("Error creating transcription for url: {Url}");
                }
            });

            Log.Information("TranscriptionsWorker started");
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Transcription service failed to start");
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
                path: "logs/all/transcription-service-all.log",
                rollingInterval: RollingInterval.Day,
                shared: true)
            .WriteTo.Logger(lc => lc
                .Enrich.FromLogContext()
                .Filter.ByExcluding(Matching.FromSource("System"))
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .WriteTo.File(
                    path: "logs/transcription-service.log",
                    rollingInterval: RollingInterval.Day,
                    shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"))
            .CreateLogger();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSerilog();
        services.AddHttpClient();
        
        var databaseServiceUrl = Environment.GetEnvironmentVariable("DATABASE_SERVICE_URL") ??
                                      throw new InvalidOperationException("Missing environment variable DATABASE_SERVICE_URL");

        services.AddHttpClient<DatabaseService>(client =>
        {
            client.BaseAddress = new Uri(databaseServiceUrl);
        });
        
        services.AddScoped<TranscriptionService>();
    }
}

record CreateTranscriptionRequest(string Url);