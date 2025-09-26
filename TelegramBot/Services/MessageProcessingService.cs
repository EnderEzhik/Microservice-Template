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
        
        logger.Information("Message receiving from user {UserId} ({Username})",
            fromId, message.From!.Username);
        
        if (message.Type != MessageType.Text)
        {
            logger.Information("Non-text message received from user {UserId}", fromId);
            await botClient.SendMessage(fromId, "Я тебя не понимаю");
            return;
        }

        var url = message.Text!;
        if (!url.StartsWith("https://www.youtube.com/"))
        {
            logger.Information("Invalid url: {Url}", url);
            await botClient.SendMessage(fromId, "Неверная ссылка");
            return;
        }

        // Убираем все параметры строки, чтобы оставить только уникальный url видео
        url = url.Split("&").First();
        
        var transcription = await transcriptionService.ProcessTranscription(url);
        if (transcription == null)
        {
            await botClient.SendMessage(fromId, "Что-то пошло не так");
            return;
        }
        await botClient.SendMessage(fromId, transcription);
    }
}