using System.Security.Cryptography;
using System.Text;

using ChatApp.Data;
using ChatApp.Models;
using ChatApp.Services;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly TokenService _token;

        public AuthController(AppDbContext db, TokenService token)
        {
            _db = db; _token = token;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] LoginDto dto)
        {
            if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
                return BadRequest("Usuário já existe");

            using var hmac = new HMACSHA256();
            var user = new User
            {
                Username = dto.Username,
                PasswordSalt = hmac.Key,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password))
            };

            _db.Users.Add(user);
            await _db.SaveChangesAsync();

            return Ok(new { token = _token.CreateToken(user.Username) });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _db.Users.SingleOrDefaultAsync(u => u.Username == dto.Username);
            if (user is null) return Unauthorized("Credenciais inválidas");

            using var hmac = new HMACSHA256(user.PasswordSalt);
            var computed = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));
            if (!computed.SequenceEqual(user.PasswordHash))
                return Unauthorized("Credenciais inválidas");

            return Ok(new { token = _token.CreateToken(user.Username) });
        }
    }

    public record LoginDto(string Username, string Password);

}
