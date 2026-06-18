using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Optivio.ServiceAbstraction.IServices;

namespace Optivio.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        private readonly IAiService _aiService;

        public AiController(IAiService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("analyze-face")]
        [Authorize]
        public async Task<IActionResult> AnalyzeFace()
        {
            var image = Request.Form.Files.FirstOrDefault();

            if (image is null || image.Length == 0)
                return BadRequest(new { message = "No image provided" });

            var allowedTypes = new[] { "image/jpeg", "image/png", "image/webp" };
            if (!allowedTypes.Contains(image.ContentType))
                return BadRequest(new { message = "Only JPEG, PNG, WEBP are allowed" });

            if (image.Length > 10 * 1024 * 1024)
                return BadRequest(new { message = "Image size must be under 10MB" });

            var result = await _aiService.AnalyzeAndRecommendAsync(
                image.OpenReadStream(),
                image.FileName
            );

            return Ok(result);
        }

        [HttpPost("try-on")]
        [Authorize]
        public async Task<IActionResult> TryOn(IFormFile userImage, [FromForm] string glassesUrl)
        {
            try
            {
                var result = await _aiService.TryOnAsync(userImage, glassesUrl);
                return File(result, "image/png");
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}