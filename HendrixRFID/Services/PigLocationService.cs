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

    public async Task ProcessScanAsync(string pigId, string lampId, int signalStrength)
    {
        // 1. Slå lampe op — vi skal bruge PlacedIn
        var lamp = await _db.Lamps.FindAsync(lampId);
        if (lamp is null)
        {
            _logger.LogWarning("Ukendt lampe: {LampId} — scanning ignoreres", lampId);
            return;
        }

        // 2. Find eller opret gris
        var pig = await _pigService.GetOrCreateAsync(pigId, lamp.PlacedIn);

        // 3. Gem RawScan — altid
        _db.RawScans.Add(new RawScan
        {
            PigId          = pigId,
            LampId         = lampId,
            SignalStrength = signalStrength,
            ScanTime       = DateTime.UtcNow
        });

        // 4. Opdater PigLocation
        var location = await _db.PigLocations.FindAsync(pigId);
        if (location is null)
        {
            // Første gang grisen ses
            _db.PigLocations.Add(new PigLocation
            {
                PigId         = pigId,
                CurrentLampId = lampId,
                LastUpdated   = DateTime.UtcNow
            });

            _logger.LogInformation("Første scanning af {PigId} ved {LampId}", pigId, lampId);
        }
        else if (location.CurrentLampId != lampId)
        {
            // Grisen er skiftet til en ny lampe — skriv historik
            _db.PigHistories.Add(new PigHistory
            {
                PigId   = pigId,
                FromLampId = location.CurrentLampId,
                LampId  = lampId,
                MovedAt = DateTime.UtcNow
            });

            location.CurrentLampId = lampId;
            location.LastUpdated   = DateTime.UtcNow;

            _logger.LogInformation("Gris {PigId} skiftet fra {FromLampId} til {LampId}", pigId, location.CurrentLampId, lampId);
        }

        await _db.SaveChangesAsync();
    }
}