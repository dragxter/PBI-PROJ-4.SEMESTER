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

### Seed database med grunddata

Databasen skal have stalde og lamper oprettet før systemet kan modtage scanninger.
Grise oprettes automatisk første gang de scannes.

Åbn `hendrix.db` i [DB Browser for SQLite](https://sqlitebrowser.org/) og kør følgende SQL under fanen "Execute SQL":

```sql
-- Opret stalde (én per fysisk bygning)
INSERT INTO Stables (StableId) VALUES (1);
INSERT INTO Stables (StableId) VALUES (2);

-- Opret lamper (LampId skal matche LAMP_ID i Arduino koden)
INSERT INTO Lamps (LampId, PlacedIn) VALUES ('lampe-01', 1);
INSERT INTO Lamps (LampId, PlacedIn) VALUES ('lampe-02', 1);
INSERT INTO Lamps (LampId, PlacedIn) VALUES ('lampe-03', 2);
```

Tryk **Execute** og derefter **Write Changes** for at gemme.

Tilføj én række per M5Stack enhed. `PlacedIn` skal pege på den stald lampen fysisk sidder i.

### Start backend

```bash
dotnet run
```

Swagger UI er tilgængeligt på http://localhost:5000/swagger

---

## Arduino konfiguration

Åbn `Arduino.txt` og tilpas disse linjer øverst i filen:

```cpp
const char* WIFI_SSID     = "wifi navn";
const char* WIFI_PASSWORD = "wifi kode";
const char* MQTT_BROKER   = "192.168.."; // IP på din PC - find den med ipconfig
const char* LAMP_ID       = "lampe-01";  // Unikt ID per M5Stack enhed
```

Find din PC's IP-adresse ved at køre `ipconfig` i en terminal og kigge efter IPv4-adressen. M5Stack og din PC skal være på det samme WiFi-netværk.

Har du flere M5Stack enheder skal hver have sit eget unikke `LAMP_ID`, fx `lampe-01`, `lampe-02` osv. Husk at `LAMP_ID` skal matche det der er indsat i databasen.

### Nødvendige Arduino biblioteker

Installer disse via Library Manager i Arduino IDE:

- PubSubClient af knolleary
- ArduinoJson af Benoit Blanchon

---

## Test uden M5Stack

Du kan teste systemet uden fysisk hardware ved at sende en MQTT besked manuelt:

```bash
mosquitto_pub -t "hendrix/scans/lampe-01" -m "{\"lampId\":\"lampe-01\",\"tags\":[{\"epcHex\":\"3074257BF7194E4000003039\",\"signalStrength\":-45}]}"
```

Tjek derefter i DB Browser at der er oprettet rækker i `RawScans`, `Pigs` og `PigLocations`.

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
    | Dekoder EPC hex → PigId via EartagDecoder
    v
PigLocationService
    | Opretter gris hvis ukendt
    | Gemmer RawScan
    | Opdaterer PigLocation
    | Skriver PigHistory ved lampeskift
    v
SQLite (hendrix.db)
```

---

## Projektstruktur

`Models` — C# klasser der repræsenterer databasetabellerne (Stable, Pig, Lamp, PigLocation, PigHistory, RawScan).

`Data` — AppDbContext, forbindelsen til SQLite via EF Core.

`DTOs` — Beskriver hvad M5Stack sender over MQTT som JSON.

`Services` —
- `MqttListenerService` lytter på MQTT og skriver til en in-memory channel
- `ScanProcessorService` læser fra channelen og koordinerer behandling
- `EartagDecoder` oversætter EPC hex-strenge til læselige PigIds
- `PigService` finder eller opretter grise i databasen
- `PigLocationService` gemmer scanninger og opdaterer grisens lokation

`Controllers` — REST API endpoints til at hente data ud.

---

## Databasestruktur

| Tabel | Formål |
|---|---|
| `Stables` | Fysiske bygninger — oprettes manuelt |
| `Lamps` | M5Stack enheder med staldtilknytning — oprettes manuelt |
| `Pigs` | Grise — oprettes automatisk ved første scanning |
| `PigLocations` | Aktuel lampe per gris — opdateres løbende |
| `PigHistories` | Log over lampeskift — én række per skift |
| `RawScans` | Alle scanninger — komplet log |
