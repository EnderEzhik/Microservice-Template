using Serilog;
using Shared.Models;

namespace Auth.Services;

public class PaymentService
{
    private readonly Serilog.ILogger logger;
    private readonly DatabaseService databaseService;

    public PaymentService(DatabaseService _databaseService)
    {
        logger = Log.ForContext<PaymentService>();
        databaseService = _databaseService;
    }

    public async Task<bool> CheckPendingPayment(long userId)
    {
        throw new NotImplementedException();
    }

    public async Task<string> CreatePayment(long userId)
    {
        throw new NotImplementedException();
    }
}