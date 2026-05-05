namespace HendrixRFID.DTOs;

// Den JSON-pakke M5Stack sender over MQTT ved hver scanning
public class MqttScanMessage
{
    public string LampId { get; set; } = string.Empty;
    public List<ScannedTag> Tags { get; set; } = new();
}

// Et enkelt tag fra scanningen
// Electronic Product Code
public class ScannedTag
{
    public string EpcHex { get; set; } = string.Empty;
    public int SignalStrength { get; set; }
}