using Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Auth.Controllers;

[ApiController]
[Route("[controller]")]
public class WebhookController : ControllerBase
{
    private readonly Serilog.ILogger logger;

    public WebhookController()
    {
        logger = Log.ForContext<WebhookController>();
    }

    [HttpPost]
    public async Task<IActionResult> Webhook(SubscriptionService subscriptionService)
    {
        throw new NotImplementedException();
    }
}