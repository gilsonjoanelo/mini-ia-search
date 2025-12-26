using ChatApp.Data;
using ChatApp.Models;
// Hubs/ChatHub.cs
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly AppDbContext _db;
        public ChatHub(AppDbContext db) => _db = db;

        // Mapear Username como UserIdentifier
        public override async Task OnConnectedAsync()
        {
            var username = Context.User!.Identity!.Name!;
            var userId = await _db.Users.Where(u => u.Username == username).Select(u => u.Id).FirstAsync();

            // Entrar em grupos de conversas que participa
            var convIds = await _db.ConversationParticipants
                .Where(p => p.UserId == userId)
                .Select(p => p.ConversationId).ToListAsync();

            foreach (var id in convIds)
                await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(id));

            // Presença
            await Clients.All.SendAsync("PresenceChanged", username, true, DateTime.UtcNow);

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? ex)
        {
            var username = Context.User!.Identity!.Name!;
            await Clients.All.SendAsync("PresenceChanged", username, false, DateTime.UtcNow);
            await base.OnDisconnectedAsync(ex);
        }

        public async Task StartTyping(int conversationId)
        {
            await Clients.Group(GroupName(conversationId))
                .SendAsync("Typing", conversationId, Context.User!.Identity!.Name!, true);
        }

        public async Task StopTyping(int conversationId)
        {
            await Clients.Group(GroupName(conversationId))
                .SendAsync("Typing", conversationId, Context.User!.Identity!.Name!, false);
        }

        public async Task SendMessage(int conversationId, string content, string? mediaUrl = null)
        {
            var username = Context.User!.Identity!.Name!;
            var senderId = await _db.Users.Where(u => u.Username == username).Select(u => u.Id).FirstAsync();

            var msg = new Message
            {
                ConversationId = conversationId,
                SenderId = senderId,
                Content = content,
                MediaUrl = mediaUrl,
                Status = MessageStatus.Sent
            };
            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            // Emitir para grupo
            await Clients.Group(GroupName(conversationId))
                .SendAsync("MessageReceived", conversationId, new
                {
                    id = msg.Id,
                    sender = username,
                    content = msg.Content,
                    mediaUrl = msg.MediaUrl,
                    at = msg.SentAt,
                    status = msg.Status.ToString()
                });
        }

        // Atualiza status para Delivered quando algum participante da conversa conectada recebe
        public async Task MarkDelivered(int conversationId, int messageId)
        {
            var msg = await _db.Messages.FindAsync(messageId);
            if (msg is null || msg.ConversationId != conversationId) return;
            msg.Status = MessageStatus.Delivered;
            await _db.SaveChangesAsync();

            await Clients.Group(GroupName(conversationId))
                .SendAsync("MessageStatusChanged", conversationId, messageId, "Delivered");
        }

        // Atualiza status para Read e LastReadAt do leitor
        public async Task MarkRead(int conversationId)
        {
            var username = Context.User!.Identity!.Name!;
            var userId = await _db.Users.Where(u => u.Username == username).Select(u => u.Id).FirstAsync();

            var unread = await _db.Messages
                .Where(m => m.ConversationId == conversationId && m.SenderId != userId && m.Status != MessageStatus.Read)
                .ToListAsync();

            foreach (var m in unread) m.Status = MessageStatus.Read;

            var participant = await _db.ConversationParticipants
                .FirstAsync(p => p.ConversationId == conversationId && p.UserId == userId);
            participant.LastReadAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            await Clients.Group(GroupName(conversationId))
                .SendAsync("MessagesRead", conversationId, username, participant.LastReadAt);
        }

        private static string GroupName(int conversationId) => $"conv:{conversationId}";
    }


}
