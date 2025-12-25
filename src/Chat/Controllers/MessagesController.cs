using ChatApp.Data;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class MessagesController : ControllerBase
    {
        private readonly AppDbContext _db;
        public MessagesController(AppDbContext db) => _db = db;

        [HttpGet("public")]
        public async Task<IActionResult> GetPublic()
        {
            var messages = await _db.Messages
                .Where(m => m.Recipient == null)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
            return Ok(messages);
        }

        [HttpGet("private/{user}")]
        public async Task<IActionResult> GetPrivate(string user)
        {
            var me = User.Identity!.Name!;
            var messages = await _db.Messages
                .Where(m => (m.Sender == me && m.Recipient == user) ||
                            (m.Sender == user && m.Recipient == me))
                .OrderBy(m => m.SentAt)
                .ToListAsync();
            return Ok(messages);
        }
    }

}
