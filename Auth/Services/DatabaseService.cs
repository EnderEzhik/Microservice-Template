using Serilog;

namespace Auth.Services;

public class DatabaseService
{
    private readonly Serilog.ILogger logger;
    private readonly HttpClient httpClient;

    public DatabaseService(HttpClient _httpClient)
    {
        logger = Log.ForContext<DatabaseService>();
        httpClient = _httpClient;
    }

    public async Task<bool> CheckSubscription(long userId)
    {
        throw new NotImplementedException();
    }

    public async Task CreateSubscription(Shared.Models.Subscription subscription)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CheckPendingPayment(long userId)
    {
        throw new NotImplementedException();
    }

    public async Task CreatePayment(Shared.Models.Payment payment)
    {
        throw new NotImplementedException();
    }

    public async Task<Shared.Models.Payment> GetPayment(string paymentGuid)
    {
        throw new NotImplementedException();
    }
}