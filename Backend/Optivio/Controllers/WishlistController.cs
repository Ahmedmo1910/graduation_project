using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optivio.ServiceAbstraction.IServices;
using System.Security.Claims;

namespace Optivio.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class WishlistController : ControllerBase
    {
        private readonly IWishlistService _wishlistService;

        public WishlistController(IWishlistService wishlistService)
        {
            _wishlistService = wishlistService;
        }

        private int GetUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> GetWishlist()
        {
            var result = await _wishlistService.GetWishlistAsync(GetUserId());
            return Ok(result);
        }

        [HttpPost("{productId}")]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            try
            {
                await _wishlistService.AddToWishlistAsync(GetUserId(), productId);
                return Ok(new { message = "Product added to wishlist" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{productId}")]
        public async Task<IActionResult> RemoveFromWishlist(int productId)
        {
            try
            {
                await _wishlistService.RemoveFromWishlistAsync(GetUserId(), productId);
                return Ok(new { message = "Product removed from wishlist" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}