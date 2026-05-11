// src/data/mockData.js

export const kpiData = {
  activePigs: 1245,
  onlineLamps: 60,
  systemUptime: "99.8%",
};

export const activityData = [
  { time: "00:00", activity: 120 },
  { time: "02:00", activity: 80 },
  { time: "04:00", activity: 60 },
  { time: "06:00", activity: 300 },
  { time: "08:00", activity: 500 },
  { time: "10:00", activity: 650 },
  { time: "12:00", activity: 700 },
  { time: "14:00", activity: 680 },
  { time: "16:00", activity: 600 },
  { time: "18:00", activity: 450 },
  { time: "20:00", activity: 250 },
  { time: "22:00", activity: 150 },
];

export const liveStatusData = [
  { id: 1, pigId: "H-999-8291", lampId: "LAMP_001", timestamp: "2026-05-09 13:40:12", status: "Aktiv" },
  { id: 2, pigId: "H-999-4412", lampId: "LAMP_003", timestamp: "2026-05-09 13:38:45", status: "Inaktiv" },
  { id: 3, pigId: "H-999-9102", lampId: "LAMP_001", timestamp: "2026-05-09 13:35:10", status: "Aktiv" },
  { id: 4, pigId: "H-999-1123", lampId: "LAMP_002", timestamp: "2026-05-09 13:30:05", status: "Aktiv" },
  { id: 5, pigId: "H-999-5531", lampId: "LAMP_004", timestamp: "2026-05-09 13:28:55", status: "Aktiv" },
  { id: 6, pigId: "H-999-7721", lampId: "LAMP_002", timestamp: "2026-05-09 13:25:20", status: "Inaktiv" },
];

export const staldData = [
  { id: "S1", name: "Stald 1" },
  { id: "S2", name: "Stald 2" },
  { id: "S3", name: "Stald 3" },
  { id: "S4", name: "Stald 4" },
];

export const lampsData = Array.from({ length: 15 }, (_, i) => ({
  id: `LAMP_${(i + 1).toString().padStart(3, '0')}`,
  status: Math.random() > 0.1 ? "Aktiv" : "Inaktiv",
  stald: `Stald ${Math.floor(Math.random() * 4) + 1}`,
  pigCount: Math.floor(Math.random() * 20) + 5,
}));

export const pigsData = Array.from({ length: 50 }, (_, i) => ({
  id: `H-999-${(1000 + i).toString()}`,
  status: Math.random() > 0.2 ? "Aktiv" : "Inaktiv",
  stald: `Stald ${Math.floor(Math.random() * 4) + 1}`,
  location: `LAMP_${(Math.floor(Math.random() * 15) + 1).toString().padStart(3, '0')}`,
  lastMoved: new Date(Date.now() - Math.floor(Math.random() * 10000000)).toLocaleString("da-DK", {
    year: "numeric", month: "2-digit", day: "2-digit", hour: "2-digit", minute: "2-digit"
  }),
}));
