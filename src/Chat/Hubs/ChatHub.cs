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

        public async Task SendToAll(string message)
        {
            var sender = Context.User!.Identity!.Name!;
            var msg = new Message { Content = message, Sender = sender };
            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            await Clients.All.SendAsync("ReceiveMessage", sender, message, msg.SentAt);
        }

        public async Task SendToUser(string recipient, string message)
        {
            var sender = Context.User!.Identity!.Name!;
            var msg = new Message { Content = message, Sender = sender, Recipient = recipient };
            _db.Messages.Add(msg);
            await _db.SaveChangesAsync();

            await Clients.User(recipient).SendAsync("ReceivePrivateMessage", sender, message, msg.SentAt);
            await Clients.User(sender).SendAsync("ReceivePrivateMessage", sender, message, msg.SentAt); // eco para quem enviou
        }

        public override Task OnConnectedAsync()
        {
            // Pode mapear UserIdentifier = Username via Claims no JWT
            return base.OnConnectedAsync();
        }
    }

}
