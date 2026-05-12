using HendrixRFID.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HendrixRFID.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PigsController : ControllerBase
{
    private readonly AppDbContext _db;

    public PigsController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> GetPigs()
    {
        var pigs = await _db.PigLocations
            .Include(pl => pl.Pig)
            .Select(pl => new {
                id = pl.PigId,
                status = "Aktiv",
                stald = $"Stald {pl.Pig.BelongsTo}",
                location = pl.CurrentLampId,
                lastMoved = pl.LastUpdated.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync();

        return Ok(pigs);
    }

    [HttpGet("{pigId}/history")]
    public async Task<IActionResult> GetPigHistory(string pigId)
    {
        var history = await _db.PigHistories
            .Where(h => h.PigId == pigId)
            .OrderByDescending(h => h.MovedAt)
            .Select(h => new {
                from = string.IsNullOrEmpty(h.FromLampId) ? "Ukendt" : h.FromLampId,
                to = h.LampId,
                timestamp = h.MovedAt.ToString("yyyy-MM-dd HH:mm:ss")
            })
            .ToListAsync();

        return Ok(history);
    }
}
