using ChatApp.Data;
using ChatApp.Models;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ConversationsController : ControllerBase
    {
        private readonly AppDbContext _db;
        public ConversationsController(AppDbContext db) => _db = db;

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateConversationDto dto)
        {
            // Suporte a 1-1: participantes = [me, alvo]
            var meName = User.Identity!.Name!;
            var me = await _db.Users.SingleAsync(u => u.Username == meName);
            var other = await _db.Users.SingleAsync(u => u.Username == dto.OtherUsername);

            // evita duplicadas 1-1
            var existing = await _db.Conversations
                .Where(c => c.Participants.Count == 2 &&
                            c.Participants.Any(p => p.UserId == me.Id) &&
                            c.Participants.Any(p => p.UserId == other.Id))
                .FirstOrDefaultAsync();

            if (existing is not null) return Ok(new { id = existing.Id });

            var conv = new Conversation();
            _db.Conversations.Add(conv);
            _db.ConversationParticipants.Add(new ConversationParticipant { Conversation = conv, UserId = me.Id });
            _db.ConversationParticipants.Add(new ConversationParticipant { Conversation = conv, UserId = other.Id });
            await _db.SaveChangesAsync();

            return Ok(new { id = conv.Id });
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var meName = User.Identity!.Name!;
            var meId = await _db.Users.Where(u => u.Username == meName).Select(u => u.Id).FirstAsync();

            var result = await _db.Conversations
                .Where(c => c.Participants.Any(p => p.UserId == meId))
                .Select(c => new
                {
                    id = c.Id,
                    title = c.Title,
                    lastMessage = _db.Messages.Where(m => m.ConversationId == c.Id)
                        .OrderByDescending(m => m.SentAt).Select(m => m.Content).FirstOrDefault(),
                    unreadCount = _db.Messages.Count(m =>
                        m.ConversationId == c.Id &&
                        m.SenderId != meId &&
                        m.SentAt >
                            c.Participants.Where(p => p.UserId == meId)
                            .Select(p => p.LastReadAt).FirstOrDefault())
                })
                .ToListAsync();

            return Ok(result);
        }

        [HttpGet("{conversationId}/messages")]
        public async Task<IActionResult> Messages(int conversationId, [FromQuery] int page = 1, [FromQuery] int pageSize = 30)
        {
            var meName = User.Identity!.Name!;
            var meId = await _db.Users.Where(u => u.Username == meName).Select(u => u.Id).FirstAsync();

            var isParticipant = await _db.ConversationParticipants.AnyAsync(p => p.ConversationId == conversationId && p.UserId == meId);
            if (!isParticipant) return Forbid();

            var messages = await _db.Messages
                .Where(m => m.ConversationId == conversationId)
                .OrderByDescending(m => m.SentAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(m => new
                {
                    id = m.Id,
                    senderId = m.SenderId,
                    content = m.Content,
                    mediaUrl = m.MediaUrl,
                    at = m.SentAt,
                    status = m.Status.ToString()
                })
                .ToListAsync();

            messages.Reverse(); // entrega em ordem cronológica ascendente
            return Ok(messages);
        }
    }

    public record CreateConversationDto(string OtherUsername);
}
