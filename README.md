# GabrielliWF — Workflow Qualità Lamiere

Applicazione **Blazor Server (.NET 8)** per registrare e monitorare il workflow di qualità sulle lamiere per il cliente Gabrielli.

## Stack

| Layer | Tecnologia |
|---|---|
| UI | MudBlazor 8.7 |
| ORM | EF Core 8 (SQL Server) |
| Query read-only | Dapper |
| Runtime | .NET 8 / Blazor Server |

## Database

| DB | Scopo |
|---|---|
| `GabrielliWF` | Dati applicazione (impegni, workflow, step, template) |
| `Intesi-FACTORY` | Reference read-only (clienti, commesse, lamiere, attributi) |

## Avvio

```powershell
cd GabrielliWF
dotnet run
```

L'app è disponibile su `http://localhost:5000`.  
Il DB deve essere già creato tramite script SQL (non ci sono migrations EF).

## Configurazione

Editare `appsettings.json` con le stringhe di connessione corrette:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=NOMESERVER;Database=GabrielliWF;User Id=utente;Password=pass;TrustServerCertificate=true;",
    "IntesiFactory":     "Server=NOMESERVER;Database=Intesi-FACTORY;User Id=utente;Password=pass;TrustServerCertificate=true;"
  }
}
```

## Installazione database

Eseguire lo script `GabrielliWF_CreateDB.sql` su SQL Server con un utente che abbia il permesso `CREATE DATABASE`.

Se il DBA crea il database vuoto in anticipo, lo script rileva l'esistenza del DB e crea solo le tabelle — l'utente applicativo deve avere `db_owner` sul database `GabrielliWF`.

> ⚠️ Il database `Intesi-FACTORY` è read-only e deve già esistere sull'istanza SQL Server.

## Funzionalità principali

- **Monitoraggio** — dashboard con tutti gli impegni, filtri per cliente/commessa/qualità/spessore, catena step colorata
- **Avanzamento** — lista impegni aperti con step da completare, registrazione avanzamento con dialogo dedicato
- **Nuovo Impegno** — selezione lamiera da `Intesi-FACTORY`, riferimenti cliente/commessa/riga, definizione workflow
- **Template** — salvataggio e riutilizzo di sequenze di step predefinite

## Workflow step — colori

| Colore | Stato |
|---|---|
| 🟢 Verde | Eseguito |
| 🔴 Rosso | Non OK |
| 🟠 Arancio | Prossimo da eseguire |
| ⬜ Grigio chiaro | Aperto (futuro) |
| ⬛ Grigio scuro | Annullato |
