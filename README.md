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

Copiare `appsettings.Development.json.example` in `appsettings.Development.json` e impostare le stringhe di connessione:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=...;Database=GabrielliWF;...",
    "IntesiFactory":     "Server=...;Database=Intesi-FACTORY;..."
  }
}
```

> ⚠️ `appsettings.Development.json` non viene committato (vedi `.gitignore`).

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
