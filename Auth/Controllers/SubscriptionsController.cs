using Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

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
    public async Task<IActionResult> CheckSubscription(SubscriptionService subscriptionService, long userId)
    {
        throw new NotImplementedException();
    }
}