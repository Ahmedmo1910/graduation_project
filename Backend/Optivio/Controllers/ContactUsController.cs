using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optivio.ServiceAbstraction.IServices;
using Optivio.Shared.DTOs.ContactUs;
using System.Security.Claims;

namespace Optivio.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactUsController : ControllerBase
    {
        private readonly IContactUsService _contactUsService;

        public ContactUsController(IContactUsService contactUsService)
        {
            _contactUsService = contactUsService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Submit(CreateContactUsDto dto)
        {
            try
            {
                int? userId = null;
                var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (userIdClaim != null)
                    userId = int.Parse(userIdClaim);

                await _contactUsService.SubmitAsync(userId, dto);
                return Ok(new { message = "Message sent successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}