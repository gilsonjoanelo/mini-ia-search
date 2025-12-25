namespace ChatApp.Models
{
    public class Message
    {
        public int Id { get; set; }
        public string Content { get; set; } = default!;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public string Sender { get; set; } = default!;
        public string? Recipient { get; set; } // null -> sala pública
    }
}
