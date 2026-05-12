using HendrixRFID.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HendrixRFID.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StablesController : ControllerBase
{
    private readonly AppDbContext _db;

    public StablesController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetStables()
    {
        var stables = await _db.Stables
            .Select(s => new {
                id = $"S{s.StableId}",
                name = $"Stald {s.StableId}"
            })
            .ToListAsync();

        return Ok(stables);
    }
}
