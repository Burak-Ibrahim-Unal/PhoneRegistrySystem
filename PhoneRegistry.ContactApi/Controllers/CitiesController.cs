using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PhoneRegistry.Infrastructure.Data;

namespace PhoneRegistry.ContactApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : ControllerBase
{
    private readonly ContactDbContext _db;

    public CitiesController(ContactDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var cities = await _db.Cities
            .OrderBy(c => c.Name)
            .Select(c => new CityDto { Id = c.Id, Name = c.Name })
            .ToListAsync(ct);
        return Ok(cities);
    }

    public class CityDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}


