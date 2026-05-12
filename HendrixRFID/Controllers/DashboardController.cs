using HendrixRFID.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HendrixRFID.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DashboardController : ControllerBase
{
    private readonly AppDbContext _db;

    public DashboardController(AppDbContext db)
    {
        _db = db;
    }

    [HttpGet("kpi")]
    public async Task<IActionResult> GetKpi()
    {
        var activePigs = await _db.PigLocations.Select(p => p.PigId).Distinct().CountAsync();
        var onlineLamps = await _db.PigLocations.Select(p => p.CurrentLampId).Distinct().CountAsync();
        
        return Ok(new {
            activePigs = activePigs,
            onlineLamps = onlineLamps,
            systemUptime = "99.9%"
        });
    }

    [HttpGet("activity")]
    public async Task<IActionResult> GetActivity()
    {
        var cutoff = DateTime.UtcNow.AddHours(-12);
        var histories = await _db.PigHistories
            .Where(h => h.MovedAt >= cutoff)
            .ToListAsync();

        var grouped = histories
            .GroupBy(h => h.MovedAt.Hour)
            .Select(g => new {
                time = $"{g.Key:D2}:00",
                activity = g.Count()
            })
            .OrderBy(x => x.time)
            .ToList();

        return Ok(grouped);
    }

    [HttpGet("live-status")]
    public async Task<IActionResult> GetLiveStatus()
    {
        var recentHistories = await _db.PigHistories
            .OrderByDescending(h => h.MovedAt)
            .Take(10)
            .Select(h => new {
                id = h.HistoryId,
                pigId = h.PigId,
                lampId = h.LampId,
                timestamp = h.MovedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                status = "Flyttet"
            })
            .ToListAsync();

        if (recentHistories.Count < 10)
        {
            var activeLocations = await _db.PigLocations
                .OrderByDescending(l => l.LastUpdated)
                .Take(10)
                .Select(l => new {
                    id = Math.Abs(l.PigId.GetHashCode()),
                    pigId = l.PigId,
                    lampId = l.CurrentLampId,
                    timestamp = l.LastUpdated.ToString("yyyy-MM-dd HH:mm:ss"),
                    status = "Registreret"
                })
                .ToListAsync();
            
            return Ok(recentHistories.Any() ? recentHistories : activeLocations);
        }

        return Ok(recentHistories);
    }
}
