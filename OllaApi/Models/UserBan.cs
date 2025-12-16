using OllaApi.Models.OllaApi.Models;

namespace OllaApi.Models
{
    public class UserBan
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public string Reason { get; set; } = "";
        public DateTime BannedUntil { get; set; } // null = навсегда
        public DateTime BannedAt { get; set; } = DateTime.UtcNow;
    }
}
