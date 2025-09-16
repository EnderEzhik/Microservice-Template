using Auth.Services;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Auth.Controllers;

[ApiController]
[Route("[controller]")]
public class PaymentsController : ControllerBase
{
    private readonly Serilog.ILogger logger;

    public PaymentsController()
    {
        logger = Log.ForContext<PaymentsController>();
    }

    [HttpGet("check/{userId:long}")]
    public async Task<IActionResult> CheckPayment(PaymentService paymentService, long userId)
    {
        throw new NotImplementedException();
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreatePayment(PaymentService paymentService)
    {
        throw new NotImplementedException();
    }
}