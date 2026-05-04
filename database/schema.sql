-- 1. Opret masterdata for stalde (Simuleret API data)
CREATE TABLE stables (
    stableId INTEGER PRIMARY KEY
);

-- 2. Opret masterdata for grise (Simuleret API data)
CREATE TABLE pigs (
    pigId TEXT PRIMARY KEY,
    belongsTo INTEGER,
    FOREIGN KEY (belongsTo) REFERENCES stables (stableId)
);

-- 3. Opret masterdata for lamper
CREATE TABLE lamps (
    lampId TEXT PRIMARY KEY,
    placedIn INTEGER,
    FOREIGN KEY (placedIn) REFERENCES stables (stableId)
);

-- 4. Opret "State" tabellen for aktuelle lokationer
-- Bemærk: pigId er Primary Key for at sikre, at en gris kun har én aktuel lokation ad gangen.
CREATE TABLE pigLocations (
    pigId TEXT PRIMARY KEY,
    currentLampId TEXT,
    lastUpdated DATETIME,
    FOREIGN KEY (pigId) REFERENCES pigs (pigId),
    FOREIGN KEY (currentLampId) REFERENCES lamps (lampId)
);

-- 5. Opret "Log" tabellen for alt rå data fra MQTT
CREATE TABLE rawScans (
    scanId INTEGER PRIMARY KEY AUTOINCREMENT,
    pigId TEXT,
    lampId TEXT,
    RSSI INTEGER,
    scanTime DATETIME,
    FOREIGN KEY (pigId) REFERENCES pigs (pigId),
    FOREIGN KEY (lampId) REFERENCES lamps (lampId)
);