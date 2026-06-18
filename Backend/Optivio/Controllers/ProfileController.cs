using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optivio.ServiceAbstraction.IServices;
using Optivio.Shared.DTOs.User;
using System.Security.Claims;

namespace Optivio.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var result = await _profileService.GetProfileAsync(GetUserId());
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile(UpdateProfileDto dto)
        {
            try
            {
                var result = await _profileService.UpdateProfileAsync(GetUserId(), dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("addresses")]
        public async Task<IActionResult> GetAddresses()
        {
            var result = await _profileService.GetAddressesAsync(GetUserId());
            return Ok(result);
        }

        [HttpPost("addresses")]
        public async Task<IActionResult> AddAddress(CreateAddressDto dto)
        {
            try
            {
                var result = await _profileService.AddAddressAsync(GetUserId(), dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("addresses/{addressId}")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            try
            {
                await _profileService.DeleteAddressAsync(GetUserId(), addressId);
                return Ok(new { message = "Address deleted" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("addresses/{addressId}")]
        public async Task<IActionResult> UpdateAddress(int addressId, CreateAddressDto dto)
        {
            try
            {
                var result = await _profileService.UpdateAddressAsync(GetUserId(), addressId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}