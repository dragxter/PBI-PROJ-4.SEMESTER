# Hendrix RFID Backend

Backend til modtagelse og lagring af RFID-scanninger fra M5Stack enheder i stalden.

---

## Forudsætninger

- .NET 9 SDK
- Mosquitto (MQTT broker)
- Arduino IDE med M5Stack board installeret

---

## Mosquitto opsætning

Mosquitto er den lokale MQTT broker som M5Stack sender data til.

### Installation

Download og installer fra https://mosquitto.org/download/

### Konfiguration

Mosquitto tillader som standard kun forbindelser fra samme PC. Det skal ændres så M5Stack kan forbinde.

Åbn filen `C:\Program Files\mosquitto\mosquitto.conf` i Notepad som administrator og tilføj disse to linjer i bunden:

```
listener 1883 0.0.0.0
allow_anonymous true
```

### Start og stop

```bash
net start mosquitto
net stop mosquitto
```

### Test at det virker

Åbn to terminaler og kør:

Terminal 1 (lyt):
```bash
mosquitto_sub -t "hendrix/scans/#"
```

Terminal 2 (send):
```bash
mosquitto_pub -t "hendrix/scans/lampe-01" -m "test"
```

Hvis "test" dukker op i terminal 1 virker Mosquitto.

---

## Backend opsætning

### Installer afhængigheder

```bash
dotnet restore
```

### Opret database

```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### Start backend

```bash
dotnet run
```

Swagger UI er tilgængeligt på http://localhost:5000/swagger

---

## Arduino konfiguration

Åbn `arduino_mqtt/rfid_scanner_mqtt.ino` og tilpas disse linjer øverst i filen:

```cpp
const char* WIFI_SSID     = "DIT_WIFI_NAVN";
const char* WIFI_PASSWORD = "DIT_WIFI_KODEORD";
const char* MQTT_BROKER   = "192.168.1.100"; // IP på din PC - find den med ipconfig
const char* LAMP_ID       = "lampe-01";      // Unikt ID per M5Stack enhed
```

Find din PC's IP-adresse ved at køre `ipconfig` i en terminal og kigge efter IPv4-adressen. M5Stack og din PC skal være på det samme WiFi-netværk.

Har du flere M5Stack enheder skal hver have sit eget unikke `LAMP_ID`, fx `lampe-01`, `lampe-02` osv.

### Nødvendige Arduino biblioteker

Installer disse via Library Manager i Arduino IDE:

- PubSubClient af knolleary
- ArduinoJson af Benoit Blanchon

---

## Arkitektur

```
M5Stack (RFID scanner)
    | WiFi / JSON over MQTT
    v
Mosquitto MQTT Broker (localhost:1883)
    | topic: hendrix/scans/{lampId}
    v
MqttListenerService (BackgroundService)
    | In-memory Channel
    v
ScanProcessorService (BackgroundService)
    | Skriver til database
    v
SQLite (hendrix.db)
```

---

## Projektstruktur

`Models` — C# klasser der repræsenterer databasetabellerne (Pig, Lamp, RawScan).

`Data` — AppDbContext, forbindelsen til SQLite via EF Core.

`DTOs` — Beskriver hvad M5Stack sender over MQTT som JSON.

`Services` — MqttListenerService lytter på MQTT og skriver til en in-memory channel. ScanProcessorService læser fra channelen og gemmer i databasen.

`Controllers` — REST API endpoints til at hente data ud.
