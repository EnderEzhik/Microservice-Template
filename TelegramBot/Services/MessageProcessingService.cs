using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Services;

public class MessageProcessingService(TranscriptionService transcriptionService)
{
    private readonly ILogger logger = Log.ForContext<MessageProcessingService>();

    public async Task ProcessAsync(ITelegramBotClient botClient, Message message)
    {
        var fromId = message.From!.Id;
        if (message.Type != MessageType.Text)
        {
            logger.Information("Non-text message received from user {UserId}", fromId);
            await botClient.SendMessage(fromId, "Я тебя не понимаю");
            return;
        }
        
        logger.Information("Message receiving from user {UserId} ({Username})",
            fromId, message.From!.Username);

        var url = message.Text!;
        if (!url.StartsWith("https://www.youtube.com/"))
        {
            logger.Information("Invalid url: {Url}", url);
            await botClient.SendMessage(fromId, "Неверная ссылка");
            return;
        }

        // Убираем все параметры строки, чтобы url видео был всегда одинаковым и уникальным  
        // Например, метка времени просмотра может всегда быть разной при одинаковой ссылке на видео
        url = url.Split("&").First();
        
        var transcription = await transcriptionService.ProcessTranscription(url);
        await botClient.SendMessage(fromId, transcription);
    }
}