namespace HendrixRFID.Models;
using System.ComponentModel.DataAnnotations;

/// <summary>
/// Repræsenterer en gris identificeret via øremærke
/// </summary>
public class Pig
{
    public string PigId { get; set; } = string.Empty;
}

/// <summary>
/// Repræsenterer en M5Stack RFID-scanner/lampe i stalden
/// </summary>
public class Lamp
{
    public string LampId { get; set; } = string.Empty;
}

/// <summary>
/// En enkelt rå scanning - logbog over hvad der er sket.
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
}