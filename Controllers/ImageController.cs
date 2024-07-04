using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PRN_ProjectAPI.Configs;
using PRN_ProjectAPI.Models;
using System.Net.Mime;

namespace PRN_ProjectAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly SystemConfig _config;

        public ImageController(SystemConfig config)
        {
            _config = config;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> GetByName(string name)
        {
            var filePath = Path.Combine(_config.UploadFilePath, name);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound("Image not found.");
            }

            try
            {
                var imageBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                var contentType = GetContentType(filePath);

                // Create a new ContentDisposition header with "inline" disposition
                var contentDisposition = new ContentDisposition
                {
                    Inline = true,
                    FileName = name // Set the filename for the browser to handle correctly
                };

                // Set the Content-Disposition header
                Response.Headers.Add("Content-Disposition", contentDisposition.ToString());

                // Return the file content
                return File(imageBytes, contentType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        private string GetContentType(string path)
        {
            var types = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase)
        {
            { ".bmp", "image/bmp" },
            { ".gif", "image/gif" },
            { ".jpeg", "image/jpeg" },
            { ".jpg", "image/jpeg" },
            { ".png", "image/png" },
            { ".tif", "image/tiff" },
            { ".tiff", "image/tiff" }
        };

            var ext = Path.GetExtension(path);
            return types.ContainsKey(ext) ? types[ext] : "application/octet-stream";
        }
    }
}
