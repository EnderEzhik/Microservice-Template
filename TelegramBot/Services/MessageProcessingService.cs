using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramBot.Services;

public class MessageProcessingService
{
    private readonly ILogger logger;

    public MessageProcessingService()
    {
        logger = Log.ForContext<MessageProcessingService>();
    }

    public async Task ProcessAsync(ITelegramBotClient botClient, Message message)
    {
        var fromId = message.From!.Id;
        
        logger.Information("Message receiving from user {UserId} ({Username})",
            fromId, message.From!.Username);
        
        await botClient.SendMessage(fromId, $"Ваше сообщение: {message.Text}");
    }
}