using OllaApi.Models.OllaApi.Models;

namespace OllaApi.Models
{
    public class Favorite
    {
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int AdId { get; set; }
        public Ad Ad { get; set; } = null!;
    }
}
