using HendrixRFID.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HendrixRFID.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LampsController : ControllerBase
{
    private readonly AppDbContext _db;

    public LampsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetLamps()
    {
        var lamps = await _db.Lamps
            .Select(l => new {
                id = l.LampId,
                status = "Aktiv",
                stald = $"Stald {l.PlacedIn}",
                pigCount = _db.PigLocations.Count(pl => pl.CurrentLampId == l.LampId)
            })
            .ToListAsync();

        return Ok(lamps);
    }
}
