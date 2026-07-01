# GabrielliWF — Workflow Qualità Lamiere

## Panoramica

Applicazione Blazor Server (.NET 8) per registrare e monitorare il workflow di qualità sulle lamiere per il cliente Gabrielli. Si appoggia a due database SQL Server sullo stesso host:

| DB | Scopo |
|---|---|
| `GabrielliWF` | Dati dell'applicazione (impegni, workflow, step, template) |
| `Intesi-FACTORY` | Reference read-only (clienti, commesse, righe ordine, rintracciabilità, attributi qualità) |

## Struttura del progetto

```
GabrielliWF/
├── GabrielliWF.sln
└── GabrielliWF/
    ├── Data/
    │   ├── AppDbContext.cs          EF Core — solo DB GabrielliWF
    │   └── Entities/                7 entità: WfStepDef, WfTemplate, WfTemplateStep,
    │                                WfImpegno, WfImpegnoRef, WfWorkflow, WfStep
    ├── Models/
    │   ├── Enums.cs                 StatoImpegno, StatoWorkflow, StatoStep
    │   ├── FactoryDtos.cs           DTO per letture da Intesi-FACTORY (Dapper)
    │   ├── ImpegnoCreateModel.cs    Form model per creazione impegno
    │   └── WorkflowModels.cs        WorkflowCreateModel, StepProgressModel
    ├── Services/
    │   ├── FactoryLookupService.cs  Tutte le query su Intesi-FACTORY (Dapper + SqlConnection)
    │   ├── ImpegnoService.cs        CRUD impegni
    │   ├── WorkflowService.cs       CRUD workflow + step + registrazione avanzamento
    │   └── TemplateService.cs       Template workflow + StepDef catalog
    └── Components/
        ├── Layout/
        │   └── MainLayout.razor     MudBlazor layout con drawer nav
        ├── Pages/
        │   ├── Index.razor          Monitoraggio impegni (dashboard con catene)
        │   ├── ImpegnoCreate.razor  Crea impegno + definisci workflow (pagina unica)
        │   ├── ImpegnoDetail.razor  Dettaglio impegno + registrazione avanzamento step
        │   └── Templates/
        │       └── TemplateList.razor  Gestione template workflow
        └── Shared/
            ├── LamieraSelector.razor         Autocomplete su L_MLPR + S_CRN
            ├── CascadeClienteCommessaRiga.razor  Cascade A_CLI → A_COM → L_CMPA
            ├── StepEditor.razor              Aggiunta/ordinamento step nel workflow
            ├── WorkflowChain.razor           Visualizzazione catena blocchi colorati
            ├── StepProgressDialog.razor      Dialog registrazione avanzamento step
            └── AddStepDialog.razor           Dialog aggiunta step a workflow aperto
```

## Schema DB (GabrielliWF)

| Tabella | Descrizione |
|---|---|
| `WF_StepDef` | Catalogo step (nome + descrizione), crescente nel tempo |
| `WF_Template` | Template di workflow salvati con nome |
| `WF_TemplateStep` | Step appartenenti a un template (ordine + IDStepDef + IDAttributo) |
| `WF_Impegno` | Impegno di una lamiera (IDCRN + quantità + stato) |
| `WF_ImpegnoRef` | Riferimenti cliente/commessa/riga per impegno (N per impegno) |
| `WF_Workflow` | Workflow associato a un impegno |
| `WF_Step` | Step di un workflow con stato + valore + nota + data prossimo step |

### Stato enum (smallint)

- `StatoImpegno`: 0=Aperto, 1=Completato, 2=Annullato
- `StatoWorkflow`: 0=Aperto, 1=Completato, 2=Annullato
- `StatoStep`: 0=Aperto, 1=Eseguito, 2=NonOk, 3=Annullato

## Letture da Intesi-FACTORY (Dapper, read-only)

Tutte le query cross-DB usano la sintassi `[Intesi-FACTORY].dbo.TableName`.

| Metodo (FactoryLookupService) | Tabelle usate |
|---|---|
| `SearchLamiereAsync` | L_MLPR + S_CRN + A_PAR + JDEQuality |
| `GetClientiAsync` | A_CLI |
| `GetCommesseByClienteAsync` | A_COM + A_CLI |
| `GetRigheByCommessaAsync` | L_CMPA |
| `GetAttributiAsync` | X_JDE_AttributiAnagrafica |
| `GetValoriAttributoAsync` | X_JDE_AttributiValori |

## Connessioni (appsettings.json)

```json
"DefaultConnection": "Server=localhost;Database=GabrielliWF;User Id=sa;Password=Intsupport1;TrustServerCertificate=true;"
"IntesiFactory":     "Server=localhost;Database=Intesi-FACTORY;User Id=sa;Password=Intsupport1;TrustServerCertificate=true;"
```

## Dipendenze principali

| Package | Versione | Uso |
|---|---|---|
| MudBlazor | 8.7.0 | UI components |
| Microsoft.EntityFrameworkCore.SqlServer | 8.0.17 | ORM per GabrielliWF DB |
| Dapper | 2.1.66 | Query read-only su Intesi-FACTORY |

## Repository GitHub

```
https://github.com/BobbyPanza/GabrielliWF
```

### Pull delle ultime modifiche

```powershell
cd c:\dev\GabrielliWF
git pull
```

### Committare e pushare modifiche

```powershell
cd c:\dev\GabrielliWF
git add -A
git commit -m "Descrizione della modifica"
git push
```

> `appsettings.Development.json` è nel `.gitignore` — non viene mai committato.

## Come avviare

```powershell
cd c:\dev\GabrielliWF\GabrielliWF
dotnet run
# oppure da Visual Studio / Rider
```

La prima esecuzione non richiede migrazioni — il DB è già creato via script SQL.

## Logica di business chiave

### Cascata cliente → commessa → riga
`A_CLI.CTCOD` → `A_COM.CTCOD` → `L_CMPA.CONUM`

### Auto-completion workflow
Quando tutti gli step di un workflow sono in stato != Aperto, il workflow passa automaticamente a Completato.

### Template
Quando si crea un impegno con "Salva come template" attivo, lo step non ancora in catalogo viene prima aggiunto a `WF_StepDef`, poi il template viene salvato con tutti i suoi step.

### Colori catena step (WorkflowChain)
- Verde (#4caf50): Eseguito
- Rosso (#f44336): NonOk
- Grigio (#9e9e9e): Annullato
- Arancio (#ff9800): Prossimo da eseguire (primo step in stato Aperto)
- Grigio chiaro (#e0e0e0): Step futuri (Aperto ma non il prossimo)
