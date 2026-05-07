namespace HendrixRFID.Models;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Repræsenterer en fysisk stald/bygning (ikke ment som indhegning)
/// </summary>
public class Stable
{
    public int StableId { get; set; }
}

/// <summary>
/// Repræsenterer en gris identificeret via øremærke.
/// BelongsTo angiver hvilken bygning grisen er registreret i.
/// </summary>
public class Pig
{
    public string PigId { get; set; } = string.Empty;

    // Hvilken bygning er grisen registreret i?
    public int BelongsTo { get; set; }
    public Stable Stable { get; set; } = null!;
}

/// <summary>
/// Repræsenterer en M5Stack RFID-scanner/lampe i stalden.
/// Er fast placeret i én bygning.
/// </summary>
public class Lamp
{
    public string LampId { get; set; } = string.Empty;

    // Hvilken bygning sidder lampen i?
    public int PlacedIn { get; set; }
    public Stable Stable { get; set; } = null!;
}

/// <summary>
/// Aktuel lokation for en gris — kun én række per gris.
/// Opdateres kun når grisen skifter lampe.
/// </summary>
public class PigLocation
{
    [Key]
    public string PigId { get; set; } = string.Empty;
    public string CurrentLampId { get; set; } = string.Empty;
    public DateTime LastUpdated { get; set; }

    public Pig Pig { get; set; } = null!;
    public Lamp Lamp { get; set; } = null!;
}

/// <summary>
/// Log over lampeskift — ny række oprettes kun når grisen skifter lampe.
/// RawScans logger alle scanninger, PigHistory logger kun skift.
/// </summary>
public class PigHistory
{
    [Key]
    public int HistoryId { get; set; }
    public string PigId { get; set; } = string.Empty;
    public string FromLampId { get; set; } = string.Empty;  // Hvilken var den ved
    public string LampId { get; set; } = string.Empty;  // hvilken lampe skiftede grisen til?
    public DateTime MovedAt { get; set; }

    public Pig Pig { get; set; } = null!;
    public Lamp Lamp { get; set; } = null!;
}

/// <summary>
/// En enkelt rå scanning - logbog over alt hvad der er sket.
/// Gemmes hver gang en lampe ser et øremærke.
/// </summary>
public class RawScan
{
    [Key]
    public int ScanId { get; set; }
    public string PigId { get; set; } = string.Empty;
    public string LampId { get; set; } = string.Empty;
    public int SignalStrength { get; set; }
    public DateTime ScanTime { get; set; } = DateTime.UtcNow;

    public Pig Pig { get; set; } = null!;
    public Lamp Lamp { get; set; } = null!;
}