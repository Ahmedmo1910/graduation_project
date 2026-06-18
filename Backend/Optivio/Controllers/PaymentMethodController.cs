using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optivio.ServiceAbstraction.IServices;
using Optivio.Shared.DTOs.Payment;
using System.Security.Claims;

namespace Optivio.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentMethodController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;

        public PaymentMethodController(IPaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetMyPaymentMethods()
        {
            var result = await _paymentMethodService.GetMyPaymentMethodsAsync(GetUserId());
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddPaymentMethod(CreatePaymentMethodDto dto)
        {
            try
            {
                var result = await _paymentMethodService.AddPaymentMethodAsync(GetUserId(), dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{paymentMethodId}")]
        public async Task<IActionResult> DeletePaymentMethod(int paymentMethodId)
        {
            try
            {
                await _paymentMethodService.DeletePaymentMethodAsync(GetUserId(), paymentMethodId);
                return Ok(new { message = "Payment method deleted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}