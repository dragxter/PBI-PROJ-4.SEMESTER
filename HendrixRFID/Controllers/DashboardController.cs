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
        var activeCutoff = DateTime.UtcNow.AddHours(-36);
        var activePigs = await _db.PigLocations
            .Where(p => p.LastUpdated >= activeCutoff)
            .Select(p => p.PigId)
            .Distinct()
            .CountAsync();
        var onlineLamps = await _db.PigLocations.Select(p => p.CurrentLampId).Distinct().CountAsync();

        var uptime = DateTime.UtcNow - System.Diagnostics.Process.GetCurrentProcess().StartTime.ToUniversalTime();
        var uptimeStr = uptime.TotalDays >= 1
            ? $"{(int)uptime.TotalDays}d {uptime.Hours}t"
            : uptime.TotalHours >= 1
                ? $"{(int)uptime.TotalHours}t {uptime.Minutes}m"
                : $"{uptime.Minutes}m";
        
        return Ok(new {
            activePigs = activePigs,
            onlineLamps = onlineLamps,
            systemUptime = uptimeStr
        });
    }

    [HttpGet("activity")]
    public async Task<IActionResult> GetActivity()
    {
        var now = DateTime.UtcNow;
        var cutoff = now.AddHours(-24);

        // RawScans is written every scan cycle — unlike PigHistories which only
        // updates at 03:00, so this is the right source for a live activity chart.
        var scans = await _db.RawScans
            .Where(r => r.ScanTime >= cutoff)
            .ToListAsync();

        // Group by the full hour slot (year+month+day+hour) to avoid midnight-crossing bugs
        var grouped = scans
            .GroupBy(r => new DateTime(r.ScanTime.Year, r.ScanTime.Month, r.ScanTime.Day, r.ScanTime.Hour, 0, 0, DateTimeKind.Utc))
            .ToDictionary(g => g.Key, g => g.Count());

        // Build all 24 hour slots so the chart always has a full x-axis even during quiet periods
        var anchorHour = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0, DateTimeKind.Utc);
        var result = Enumerable.Range(-23, 24)
            .Select(i =>
            {
                var slot = anchorHour.AddHours(i);
                return new
                {
                    time = $"{slot.Hour:D2}:00",
                    activity = grouped.TryGetValue(slot, out var count) ? count : 0
                };
            })
            .ToList();

        return Ok(result);
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
