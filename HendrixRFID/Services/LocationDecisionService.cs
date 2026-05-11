using HendrixRFID.Data;
using HendrixRFID.Models;
using Microsoft.EntityFrameworkCore;

namespace HendrixRFID.Services;

public class LocationDecisionService
{
    private readonly AppDbContext _db;
    private readonly ILogger<LocationDecisionService> _logger;

    // RSSI grænseværdier — justeres efter test i stalden
    private const int StrongSignalThreshold = -55;
    private const int WeakSignalThreshold   = -75;

    public LocationDecisionService(AppDbContext db, ILogger<LocationDecisionService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        var cutoff = DateTime.UtcNow.AddHours(-24);

        // Hent kun lokationer (dette er lille i hukommelsen)
        var allLocations = await _db.PigLocations.ToListAsync();

        foreach (var location in allLocations)
        {
            // Find scanninger for KUN denne gris
            var pigScans = await _db.RawScans
                .Where(r => r.ScanTime >= cutoff && r.PigId == location.PigId)
                .ToListAsync();

            if (!pigScans.Any())
                continue;

            // Beregn vægtet score per lampe
            var scores = new Dictionary<string, double>();

            foreach (var scan in pigScans)
            {
                if (!scores.ContainsKey(scan.LampId))
                    scores[scan.LampId] = 0;

                scores[scan.LampId] += GetWeight(scan.SignalStrength);
            }

            // Find bedste lampe
            var bestLamp  = scores.MaxBy(s => s.Value).Key;
            var bestScore = scores[bestLamp];

            // Samme lampe som nu?
            if (bestLamp == location.CurrentLampId)
                continue;

            // Nuværende lampes score
            var currentScore = scores.TryGetValue(location.CurrentLampId, out var s) ? s : 0;

            // Anvend regler
            bool shouldMove;

            if (currentScore == 0)
            {
                // Regel 1 — ingen konkurrence
                shouldMove = bestScore >= 3.0;
            }
            else
            {
                // Regel 2 — konkurrence
                shouldMove = bestScore / currentScore >= 3.0;
            }

            if (!shouldMove)
                continue;

            // Flyt grisen
            _logger.LogInformation("Gris {PigId} flyttes fra {OldLamp} til {NewLamp}",
                location.PigId, location.CurrentLampId, bestLamp);

            _db.PigHistories.Add(new PigHistory
            {
                PigId      = location.PigId,
                FromLampId = location.CurrentLampId,
                LampId     = bestLamp,
                MovedAt    = DateTime.UtcNow
            });

            location.CurrentLampId = bestLamp;
            location.LastUpdated   = DateTime.UtcNow;
        }

        await _db.SaveChangesAsync();
    }

    private double GetWeight(int rssi)
    {
        if (rssi > StrongSignalThreshold) return 2.0;
        if (rssi > WeakSignalThreshold)   return 1.0;
        return 0.5;
    }
}