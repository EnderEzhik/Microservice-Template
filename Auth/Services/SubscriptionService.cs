using Serilog;

namespace Auth.Services;

public class SubscriptionService
{
    private readonly Serilog.ILogger logger;
    private readonly DatabaseService databaseService;

    public SubscriptionService(DatabaseService _databaseService)
    {
        logger = Log.ForContext<SubscriptionService>();
        databaseService = _databaseService;
    }

    public async Task<bool> IsUserSubscribed(long userId)
    {
        logger.Information("Checking is user subscribed for user: {UserId}", userId);

        try
        {
            return await databaseService.IsUserSubscribed(userId);
        }
        catch
        {
            logger.Error("DatabaseService failed during IsUserSubscribed. UserId={UserId}", userId);
            throw;
        }
    }

    public async Task CreateSubscription(string paymentGuid)
    {
        throw new NotImplementedException();
    }
}
