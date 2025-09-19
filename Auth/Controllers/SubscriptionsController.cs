using Microsoft.AspNetCore.Mvc;
using Serilog;
using Auth.Services;

namespace Auth.Controllers;

[ApiController]
[Route("[controller]")]
public class SubscriptionsController : ControllerBase
{
    private readonly Serilog.ILogger logger;

    public SubscriptionsController()
    {
        logger = Log.ForContext<SubscriptionsController>();
    }

    [HttpGet("check/{userId:long}")]
    public async Task<ActionResult<SubscriptionStatusResponse>> CheckUserSubscription(SubscriptionService subscriptionService, long userId)
    {
        logger.Information("GET request for checking subscription for user: {UserId}", userId);

        try
        {
            var isSubscribed = await subscriptionService.IsUserSubscribed(userId);
            logger.Information("User subscription status: {IsSubscribed}", isSubscribed);

            return Ok(new SubscriptionStatusResponse
            {
                IsSubscribed = isSubscribed,
                UserId = userId
            });
        }
        catch
        {
            logger.Error("DatabaseService error during subscription check. UserId={UserId}", userId);
            throw;
        }
    }
}

public record SubscriptionStatusResponse
{
    public long UserId { get; init; }
    public bool IsSubscribed { get; init; }
}
