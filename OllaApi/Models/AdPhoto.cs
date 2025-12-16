namespace OllaApi.Models
{
    public class AdPhoto
    {
        public int Id { get; set; }
        public string Url { get; set; } = ""; // Пока просто строка, потом можно файлы
        public int AdId { get; set; }
        public Ad Ad { get; set; } = null!;
    }
}
