using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Filters;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBot.Services;

namespace TelegramBot;

class Program
{
    private static TelegramBotClient botClient;

    private static IServiceProvider serviceProvider;

    private static async Task Main(string[] args)
    {
        ConfigureLogging();

        try
        {
            Log.Information("Starting TelegramBot...");

            var builder = Host.CreateApplicationBuilder(args);

            ConfigureServices(builder.Services);

            var host = builder.Build();
            serviceProvider = host.Services;

            var botToken = Environment.GetEnvironmentVariable("BOT_TOKEN") ??
                           throw new InvalidOperationException("Missing environment variable BOT_TOKEN");

            botClient = new TelegramBotClient(botToken);
            await botClient.DropPendingUpdates();

            botClient.OnMessage += OnMessage;
            botClient.OnError += OnError;

            var me = await botClient.GetMe();
            Log.Information("Telegram Bot is ready. My username: {Username}", me.Username);

            await Task.Delay(Timeout.Infinite);
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Telegram bot failed to start");
            throw;
        }
        finally
        {
            await Log.CloseAndFlushAsync();
        }
    }

    private static void ConfigureLogging()
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.File(
                path: "logs/all/telegram-bot-all.log",
                rollingInterval: RollingInterval.Day,
                shared: true)
            .WriteTo.Logger(lc => lc
                .Enrich.FromLogContext()
                .Filter.ByExcluding(Matching.FromSource("System"))
                .Filter.ByExcluding(Matching.FromSource("Microsoft"))
                .WriteTo.File(
                    path: "logs/telegram-bot.log",
                    rollingInterval: RollingInterval.Day,
                    shared: true,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"))
            .CreateLogger();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        services.AddSerilog();
        services.AddHttpClient();

        var transcriptionServiceUrl = Environment.GetEnvironmentVariable("TRANSCRIPTION_SERVICE_URL") ??
                                      throw new InvalidOperationException("Missing environment variable TRANSCRIPTION_SERVICE_URL");

        services.AddHttpClient<TranscriptionService>(client => { client.BaseAddress = new Uri(transcriptionServiceUrl); });

        services.AddScoped<MessageProcessingService>();
    }

    private static Task OnError(Exception exception, HandleErrorSource source)
    {
        Log.Error(exception, "Bot error occured. Source: {Source}", source);
        return Task.CompletedTask;
    }

    private static async Task OnMessage(Message message, UpdateType type)
    {
        using var scope = serviceProvider.CreateScope();
        var processor = scope.ServiceProvider.GetRequiredService<MessageProcessingService>();
        await processor.ProcessAsync(botClient, message);
    }
}