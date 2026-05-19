# Hendrix Genetics RFID Dashboard

Dette er frontend-applikationen til Hendrix Genetics RFID-projektet. Dashboardet er bygget med **React** og **Vite**, og det er designet til at give et visuelt overblik over grisens bevægelser, lampestatus og live-aktivitet via det tilhørende .NET-backend API.

## Forudsætninger

Før du kan køre dette projekt, skal du sikre dig, at du har følgende installeret:
- [Node.js](https://nodejs.org/) (Version 18 eller nyere anbefales)
- `.NET 9 SDK` (Til at køre backend-API'et)

**Vigtigt:** For at dashboardet kan vise data, **skal backend-serveren køre** samtidig. Backend'en forventes at køre på `http://localhost:5167`.

## Installation

1. Åbn en terminal og naviger til `dashboard`-mappen.
2. Kør følgende kommando for at installere alle nødvendige pakker:
   ```bash
   npm install
   ```

## Kør applikationen lokalt

For at starte udviklingsserveren og se dashboardet i din browser:

1. Start først backend-API'et (i projektets rodmappe):
   ```bash
   cd HendrixRFID
   dotnet run
   ```
2. Start frontend-dashboardet (i en ny terminal i `dashboard`-mappen):
   ```bash
   npm run dev
   ```
3. Åbn din browser og gå til URL'en angivet i terminalen (typisk `http://localhost:5173`).

## API Forbindelse

Dashboardet kommunikerer med .NET API'et for at hente data. Alle HTTP-forespørgsler er samlet i filen `src/api.js`. Hvis backenden ændrer port (f.eks. ved deployment), er det kun variablen `API_BASE_URL` i denne fil, der skal opdateres.
