namespace ChatApp.Models
{
    public class Conversation
    {
        public int Id { get; set; }
        public string Title { get; set; } = ""; // opcional (para grupos)
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<ConversationParticipant> Participants { get; set; } = new List<ConversationParticipant>();
    }

}
