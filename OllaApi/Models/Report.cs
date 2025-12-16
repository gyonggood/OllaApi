namespace OllaApi.Models
{
    public class Report
    {
        public int Id { get; set; }
        public int AdId { get; set; }
        public Ad Ad { get; set; } = null!;
        public int FromUserId { get; set; }
        public string Reason { get; set; } = "";
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
