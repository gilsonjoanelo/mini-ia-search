namespace ChatApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ConversationId { get; set; }
        public Conversation Conversation { get; set; } = default!;
        public int SenderId { get; set; }
        public User Sender { get; set; } = default!;
        public string Content { get; set; } = "";
        public string? MediaUrl { get; set; } // anexos
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public MessageStatus Status { get; set; } = MessageStatus.Sent;

    }

    public enum MessageStatus
    {
        Sent,
        Delivered,
        Read
    }
}
