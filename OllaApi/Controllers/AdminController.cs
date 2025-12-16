using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OllaApi.Data;
using OllaApi.Models;

namespace OllaApi.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Policy = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly AppDbContext _db;

        public AdminController(AppDbContext db) => _db = db;

        [HttpGet("reports")]
        public async Task<IActionResult> GetReports()
        {
            var reports = await _db.Reports
                .Include(r => r.Ad)
                .ThenInclude(a => a.User)
                .Select(r => new
                {
                    r.Id,
                    r.Reason,
                    r.CreatedAt,
                    AdId = r.Ad.Id,
                    AdTitle = r.Ad.Title,
                    AdOwnerId = r.Ad.UserId
                })
                .ToListAsync();

            return Ok(reports);
        }

        [HttpDelete("ads/{id}")]
        public async Task<IActionResult> DeleteAd(int id)
        {
            var ad = await _db.Ads.FindAsync(id);
            if (ad == null) return NotFound();

            _db.Ads.Remove(ad);
            await _db.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("ban/{userId}")]
        public async Task<IActionResult> BanUser(int userId, [FromBody] BanDto dto)
        {
            _db.UserBans.Add(new UserBan
            {
                UserId = userId,
                Reason = dto.Reason,
                BannedUntil = dto.Days > 0 ? DateTime.UtcNow.AddDays(dto.Days) : DateTime.MaxValue
            });
            await _db.SaveChangesAsync();
            return Ok();
        }
    }

    public class BanDto
    {
        public string Reason { get; set; } = "";
        public int Days { get; set; } = 0;
    }
}