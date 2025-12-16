namespace OllaApi.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int AdId { get; set; }
        public int FromUserId { get; set; }
        public int ToUserId { get; set; }
        public string Text { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;
    }
}