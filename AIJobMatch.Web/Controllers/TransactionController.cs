using AIJobMatch.Application.IServices;
using AIJobMatch.Application.ViewModels.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PayOS;
using PayOS.Models.Webhooks;

namespace AIJobMatch.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TransactionController : ControllerBase
    {
        private readonly ITransactionService _transactionService;
        private readonly PayOSClient _payOS;
        public TransactionController(ITransactionService transactionService, PayOSClient payOS)
        {
            _transactionService = transactionService;
            _payOS = payOS;
        }

        [HttpGet("Get-Link-Transaciton")]
        public async Task<IActionResult> GetLinkTransaction([FromQuery] CreatePaymentRequest request)
        {
            try
            {
                var result = await _transactionService.CreatePayment(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> HandleWebhook([FromBody] Webhook webhookData)
        {
            try
            {
                var verifiedData = await _payOS.Webhooks.VerifyAsync(webhookData);
                var result = await _transactionService.VerifyWebhookSuccess(webhookData);
                return Ok("Success");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PayOS Verification Failed: {ex.ToString()}");
                return BadRequest("Invalid webhook");
            }
        }
    }
}
