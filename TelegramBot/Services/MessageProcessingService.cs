using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace TelegramBot.Services;

public class MessageProcessingService
{
    private readonly ILogger logger;
    private readonly TranscriptionService transcriptionService;

    public MessageProcessingService(TranscriptionService _transcriptionService)
    {
        logger = Log.ForContext<MessageProcessingService>();
        transcriptionService = _transcriptionService;
    }

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
            logger.Information("Invalid url");
            await botClient.SendMessage(fromId, "Неверная ссылка");
            return;
        }
        
        var transcription = await transcriptionService.ProcessTranscription(url);
        await botClient.SendMessage(fromId, transcription);
    }
}