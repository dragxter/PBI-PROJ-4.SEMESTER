using HendrixRFID.Data;
using HendrixRFID.Models;

namespace HendrixRFID.Services;

public class PigService
{
    private readonly AppDbContext _db;
    private readonly ILogger<PigService> _logger;

    public PigService(AppDbContext db, ILogger<PigService> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>
    /// Finder eller opretter en gris.
    /// Hvis grisen er ny sættes BelongsTo til lampens stald.
    /// </summary>
    public async Task<Pig> GetOrCreateAsync(string pigId, int stableId)
    {
        var pig = await _db.Pigs.FindAsync(pigId);
        if (pig is not null)
            return pig;

        pig = new Pig
        {
            PigId     = pigId,
            BelongsTo = stableId
        };

        _db.Pigs.Add(pig);
        _logger.LogInformation("Ny gris oprettet: {PigId} i stald {StableId}", pigId, stableId);

        return pig;
    }
}