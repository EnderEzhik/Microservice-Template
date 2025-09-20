using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;
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

            app.MapGet("transcription", async (MyDbContext db, [FromQuery] string url) =>
            {
                Log.Information("GET request for get transcription for url: {Url}", url);

                try
                {
                    var transcription = await db.Transcriptions.FirstOrDefaultAsync(t => t.Url == url);

                    if (transcription != null)
                    {
                        Log.Information("Transcription found for url: {Url}", url);
                        return Results.Ok(transcription);
                    }
                    
                    Log.Information("Transcription not found for url: {Url}", url);
                    return  Results.NotFound();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error getting transcription for url: {Url}", url);
                    throw;
                }
            });
            
            app.MapPost("transcription", async (MyDbContext db, CreateTranscriptionRequest request) =>
            {
                Log.Information("POST request for save transcription for url: {Url}", request.Url);

                try
                {
                    var newTranscription = new Shared.Models.Transcription()
                    {
                        Url = request.Url,
                        Content = request.Content
                    };
                    
                    db.Transcriptions.Add(newTranscription);
                    await db.SaveChangesAsync();
                    
                    Log.Information("Transcription saved to db for url: {Url}", request.Url);
                    return Results.Created();
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Error saving transcription for url: {Url}", request.Url);
                    throw;
                }
            });

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

    private static void ConfigureDatabase(WebApplicationBuilder builder)
    {
        Log.Information("Attempting to connect to the database");   
        
        var dbDefaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                                        throw new InvalidOperationException("Missing environment variable DefaultConnection");

        builder.Services.AddDbContext<MyDbContext>(options => { options.UseNpgsql(dbDefaultConnectionString); });

        Log.Information("Database connection established");
    }
}

record CreateTranscriptionRequest
{
    [JsonPropertyName("url")]
    public string Url { get; init; }
    
    [JsonPropertyName("content")]
    public string Content { get; init; }
}