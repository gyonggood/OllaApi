namespace OllaApi.Models
{
    public class AdView
    {
        public int Id { get; set; }
        public int AdId { get; set; }
        public Ad Ad { get; set; } = null!;
        public int? UserId { get; set; } // Может быть гость
        public string IpAddress { get; set; } = "";
        public DateTime ViewedAt { get; set; } = DateTime.UtcNow;
    }
}
