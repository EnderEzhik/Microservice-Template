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

    public async Task<bool> CheckSubscription(long userId)
    {
        throw new NotImplementedException();
    }

    public async Task CreateSubscription(string paymentGuid)
    {
        throw new NotImplementedException();
    }
}