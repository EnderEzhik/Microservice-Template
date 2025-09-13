using Microsoft.AspNetCore.Mvc;
using Serilog;
using Transcription.Services;

namespace Transcription;

public class Program
{
    public static void Main(string[] args)
    {
        ConfigureLogging();

        try
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog();
            
            ConfigureServices(builder.Services);

            var app = builder.Build();

            app.MapGet("transcription", async (TranscriptionService transcriptionService, [FromQuery] string url) =>
            {
                Log.Information("GET request for get transcription with url: {Url}", url);

                try
                {
                    var transcription = await transcriptionService.FindTranscription(url);
                    return transcription != null ? Results.Ok(transcription) : Results.NotFound();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error getting transcription for url: {Url}", url);
                    return Results.Problem("Error getting transcription");
                }
            });

            app.MapPost("transcription", async (TranscriptionService transcriptionService, [FromBody] CreateTranscriptionRequest request) =>
            {
                var url = request.url;
                Log.Information("POST request for create transcription with url: {Url}", url);

                try
                {
                    var transcription = await transcriptionService.CreateTranscription(url);
                    // В ответе будет сущность без установленного id
                    // Возможно стоит возвращать сгенерированный id сущности после сохранения в бд
                    // Или заменить сущность для возврата на DTO без id, так как id не будет нигде использоваться
                    return Results.Ok(transcription);
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error creating transcription for url: {Url}", url);
                    return Results.Problem("Error creating transcription");
                }
            });

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception");
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
                path: "logs/transcription-service.log",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7,
                outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj}{NewLine}{Exception}")
            .CreateLogger();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddHttpClient();
        
        var databaseServiceUrl = Environment.GetEnvironmentVariable("DATABASE_SERVICE_URL") ??
                                      throw new InvalidOperationException(
                                          "Missing environment variable DATABASE_SERVICE_URL");

        services.AddHttpClient<DatabaseService>(client =>
        {
            client.BaseAddress = new Uri(databaseServiceUrl);
        });
        
        services.AddScoped<TranscriptionService>();
    }
}

record CreateTranscriptionRequest(string url);