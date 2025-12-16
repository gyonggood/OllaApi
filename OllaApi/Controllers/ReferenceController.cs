using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OllaApi.Data;
using OllaApi.Models;

namespace OllaApi.Controllers
{
    [Route("api/ref")]
    [ApiController]
    public class ReferenceController : ControllerBase
    {
        private readonly AppDbContext _db;

        public ReferenceController(AppDbContext db) => _db = db;

        // GET api/ref/categories
        [HttpGet("categories")]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories()
        {
            var categories = await _db.Categories
                .Select(c => new CategoryDto { Id = c.Id, Name = c.Name })
                .ToListAsync();

            return Ok(categories);
        }

        // POST api/ref/categories — все могут добавлять
        [HttpPost("categories")]
        public async Task<ActionResult<CategoryDto>> AddCategory([FromBody] CategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Название категории обязательно");

            var existing = await _db.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == dto.Name.Trim().ToLower());

            if (existing != null)
                return Conflict("Категория уже существует");

            var cat = new Category { Name = dto.Name.Trim() };
            _db.Categories.Add(cat);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCategories), new CategoryDto { Id = cat.Id, Name = cat.Name });
        }

        // PUT api/ref/categories/{id} — редактирование категории (все могут)
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Название категории обязательно");

            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();

            var existing = await _db.Categories
                .FirstOrDefaultAsync(c => c.Name.ToLower() == dto.Name.Trim().ToLower() && c.Id != id);

            if (existing != null)
                return Conflict("Категория с таким названием уже существует");

            cat.Name = dto.Name.Trim();
            await _db.SaveChangesAsync();

            return Ok(new CategoryDto { Id = cat.Id, Name = cat.Name });
        }

        // DELETE api/ref/categories/{id} — удаление категории (все могут)
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var cat = await _db.Categories.FindAsync(id);
            if (cat == null) return NotFound();

            // Если есть объявления с этой категорией — нельзя удалить (или можно, но с каскадом — решай сам)
            var hasAds = await _db.Ads.AnyAsync(a => a.CategoryId == id);
            if (hasAds) return BadRequest("Нельзя удалить категорию — есть объявления");

            _db.Categories.Remove(cat);
            await _db.SaveChangesAsync();

            return Ok("Категория удалена");
        }

        // GET api/ref/cities
        [HttpGet("cities")]
        public async Task<ActionResult<List<CityDto>>> GetCities()
        {
            var cities = await _db.Cities
                .Select(c => new CityDto { Id = c.Id, Name = c.Name })
                .ToListAsync();

            return Ok(cities);
        }

        // POST api/ref/cities
        [HttpPost("cities")]
        public async Task<ActionResult<CityDto>> AddCity([FromBody] CityDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Название города обязательно");

            var existing = await _db.Cities
                .FirstOrDefaultAsync(c => c.Name.ToLower() == dto.Name.Trim().ToLower());

            if (existing != null)
                return Conflict("Город уже существует");

            var city = new City { Name = dto.Name.Trim() };
            _db.Cities.Add(city);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCities), new CityDto { Id = city.Id, Name = city.Name });
        }

        // PUT api/ref/cities/{id}
        [HttpPut("cities/{id}")]
        public async Task<IActionResult> UpdateCity(int id, [FromBody] CityDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name))
                return BadRequest("Название города обязательно");

            var city = await _db.Cities.FindAsync(id);
            if (city == null) return NotFound();

            var existing = await _db.Cities
                .FirstOrDefaultAsync(c => c.Name.ToLower() == dto.Name.Trim().ToLower() && c.Id != id);

            if (existing != null)
                return Conflict("Город с таким названием уже существует");

            city.Name = dto.Name.Trim();
            await _db.SaveChangesAsync();

            return Ok(new CityDto { Id = city.Id, Name = city.Name });
        }

        // DELETE api/ref/cities/{id}
        [HttpDelete("cities/{id}")]
        public async Task<IActionResult> DeleteCity(int id)
        {
            var city = await _db.Cities.FindAsync(id);
            if (city == null) return NotFound();

            var hasAds = await _db.Ads.AnyAsync(a => a.CityId == id);
            if (hasAds) return BadRequest("Нельзя удалить город — есть объявления");

            _db.Cities.Remove(city);
            await _db.SaveChangesAsync();

            return Ok("Город удалён");
        }
    }

    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }

    public class CityDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
    }
}