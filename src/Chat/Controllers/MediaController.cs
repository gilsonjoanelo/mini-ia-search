using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public MediaController(IWebHostEnvironment env) => _env = env;

        [HttpPost("upload")]
        [RequestSizeLimit(20_000_000)] // 20 MB
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("Arquivo vazio");
            var uploads = Path.Combine(_env.ContentRootPath, "uploads");
            Directory.CreateDirectory(uploads);
            var name = $"{Guid.NewGuid()}_{file.FileName}";
            var path = Path.Combine(uploads, name);
            await using var stream = System.IO.File.Create(path);
            await file.CopyToAsync(stream);
            // Em produção: servir via CDN/Storage; aqui, expor como estático ou via endpoint
            return Ok(new { url = $"/uploads/{name}" });
        }
    }

}
