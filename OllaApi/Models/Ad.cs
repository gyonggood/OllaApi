using OllaApi.Models.OllaApi.Models;

namespace OllaApi.Models
{
    public class Ad
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;

        public int CityId { get; set; }
        public City City { get; set; } = null!;

        public List<AdPhoto> Photos { get; set; } = new();
        public List<Favorite> Favorites { get; set; } = new();
        public List<Message> Messages { get; set; } = new();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
