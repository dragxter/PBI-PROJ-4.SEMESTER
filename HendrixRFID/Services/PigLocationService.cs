using HendrixRFID.Data;
using HendrixRFID.Models;

namespace HendrixRFID.Services;

public class PigLocationService
{
    private readonly AppDbContext _db;
    private readonly PigService _pigService;
    private readonly ILogger<PigLocationService> _logger;

    public PigLocationService(AppDbContext db, PigService pigService, ILogger<PigLocationService> logger)
    {
        _db = db;
        _pigService = pigService;
        _logger = logger;
    }

    public async Task UpdateLampHeartbeatAsync(string lampId)
    {
        var lamp = await _db.Lamps.FindAsync(lampId);
        if (lamp != null)
        {
            lamp.LastSeen = DateTime.UtcNow;
            await _db.SaveChangesAsync();
        }
    }

    public async Task ProcessScansBatchAsync(string lampId, IEnumerable<(string pigId, int signalStrength)> scans)
    {
        // 1. Slå lampe op — vi skal bruge PlacedIn
        var lamp = await _db.Lamps.FindAsync(lampId);
        if (lamp is null)
        {
            _logger.LogWarning("Ukendt lampe: {LampId} — scanning ignoreres", lampId);
            return;
        }

        foreach (var scan in scans)
        {
            // 2. Find eller opret gris
            await _pigService.GetOrCreateAsync(scan.pigId, lamp.PlacedIn);

            // 3. Gem RawScan — altid
            _db.RawScans.Add(new RawScan
            {
                PigId          = scan.pigId,
                LampId         = lampId,
                SignalStrength = scan.signalStrength,
                ScanTime       = DateTime.UtcNow
            });

            // 4. Opret PigLocation kun hvis grisen er ny, ellers opdater tidspunktet for at vise at den er i live
            var location = await _db.PigLocations.FindAsync(scan.pigId);
            if (location is null)
            {
                _db.PigLocations.Add(new PigLocation
                {
                    PigId         = scan.pigId,
                    CurrentLampId = lampId,
                    LastUpdated   = DateTime.UtcNow
                });

                _logger.LogInformation("Første scanning af {PigId} ved {LampId}", scan.pigId, lampId);
            }
            else
            {
                location.LastUpdated = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync();
    }
}