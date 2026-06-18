using Microsoft.AspNetCore.Mvc;

namespace Optivio.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;

        public FilesController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpGet("images/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var basePath = _env.WebRootPath ?? _env.ContentRootPath;
            var path = Path.Combine(basePath, "images", fileName);

            if (!System.IO.File.Exists(path))
                return NotFound(new { message = $"File not found: {path}" });

            var mimeType = fileName.EndsWith(".png") ? "image/png" :
                           fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") ? "image/jpeg" : "image/png";

            return PhysicalFile(path, mimeType);
        }

        [HttpGet("models/{fileName}")]
        public IActionResult GetModel(string fileName)
        {
            var basePath = _env.WebRootPath ?? _env.ContentRootPath;
            var path = Path.Combine(basePath, "models", fileName);

            if (!System.IO.File.Exists(path))
                return NotFound(new { message = $"File not found: {path}" });

            return PhysicalFile(path, "model/gltf-binary");
        }

        [HttpGet("images2d/{fileName}")]
        public IActionResult Get2DImage(string fileName)
        {
            var basePath = _env.WebRootPath ?? _env.ContentRootPath;
            var path = Path.Combine(basePath, "images2d", fileName);

            if (!System.IO.File.Exists(path))
                return NotFound();

            var mimeType = fileName.EndsWith(".png") ? "image/png" :
                           fileName.EndsWith(".jpg") || fileName.EndsWith(".jpeg") ? "image/jpeg" : "image/png";

            return PhysicalFile(path, mimeType);
        }
    }
}