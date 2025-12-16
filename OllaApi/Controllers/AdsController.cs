using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OllaApi.Data;
using OllaApi.Models;
using System.Security.Claims;

namespace OllaApi.Controllers
{
    [Route("api/ads")]
    [ApiController]
    [Authorize]
    public class AdsController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdsController(AppDbContext db) => _db = db;

        private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

        // GET api/ads
        [HttpGet]
        public async Task<IActionResult> GetAll(int? categoryId, int? cityId)
        {
            var query = _db.Ads
                .Include(a => a.Category)
                .Include(a => a.City)
                .Include(a => a.Photos)
                .AsQueryable();

            if (categoryId.HasValue) query = query.Where(a => a.CategoryId == categoryId.Value);
            if (cityId.HasValue) query = query.Where(a => a.CityId == cityId.Value);

            var ads = await query
                .OrderByDescending(a => a.CreatedAt)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Description,
                    a.CreatedAt,
                    a.UserId,
                    Category = a.Category.Name,
                    City = a.City.Name,
                    Photos = a.Photos.Select(p => p.Url).ToList(),
                    Views = _db.AdViews.Count(v => v.AdId == a.Id),
                    IsFavorite = _db.Favorites.Any(f => f.AdId == a.Id && f.UserId == CurrentUserId)
                })
                .ToListAsync();

            return Ok(ads);
        }

        // POST api/ads
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AdCreateDto dto)
        {
            var ad = new Ad
            {
                Title = dto.Title,
                Description = dto.Description,
                UserId = CurrentUserId,
                CategoryId = dto.CategoryId,
                CityId = dto.CityId
            };

            _db.Ads.Add(ad);
            await _db.SaveChangesAsync();

            if (dto.PhotoUrls != null && dto.PhotoUrls.Any())
            {
                foreach (var url in dto.PhotoUrls)
                {
                    _db.AdPhotos.Add(new AdPhoto { AdId = ad.Id, Url = url });
                }
                await _db.SaveChangesAsync();
            }

            return CreatedAtAction(nameof(GetAll), ad);
        }

        // PUT api/ads/{id} — обновление (только владелец)
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] AdUpdateDto dto)
        {
            var ad = await _db.Ads
                .Include(a => a.Photos)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ad == null) return NotFound();
            if (ad.UserId != CurrentUserId) return Forbid();

            ad.Title = dto.Title;
            ad.Description = dto.Description;
            ad.CategoryId = dto.CategoryId;
            ad.CityId = dto.CityId;

            // Обновляем фото
            _db.AdPhotos.RemoveRange(ad.Photos);
            if (dto.PhotoUrls != null && dto.PhotoUrls.Any())
            {
                foreach (var url in dto.PhotoUrls)
                {
                    _db.AdPhotos.Add(new AdPhoto { AdId = ad.Id, Url = url });
                }
            }

            await _db.SaveChangesAsync();
            return Ok(ad);
        }

        // DELETE api/ads/{id} — удаление (владелец или админ)
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return NotFound();

            if (ad.UserId != CurrentUserId && !User.IsInRole("Admin"))
                return Forbid();

            _db.Ads.Remove(ad);
            await _db.SaveChangesAsync();

            return Ok("Объявление удалено");
        }
    }

    public class AdCreateDto
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int CategoryId { get; set; }
        public int CityId { get; set; }
        public List<string>? PhotoUrls { get; set; }
    }

    public class AdUpdateDto
    {
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public int CategoryId { get; set; }
        public int CityId { get; set; }
        public List<string>? PhotoUrls { get; set; }
    }

    public class ReportDto
    {
        public string Reason { get; set; } = "";
    }
}