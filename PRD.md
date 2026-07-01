# PRD — GabrielliWF: Workflow Qualità Lamiere

**Cliente:** Gabrielli  
**Data:** 2026-07-01  
**Versione:** 1.0  
**Stack:** Blazor Server .NET 8 + SQL Server  

---

## 1. Obiettivo

Applicazione web per registrare e monitorare il **workflow di qualità** applicato alle lamiere (coils/piastre) prima di produrre i pezzi per i clienti. Ogni lamiera viene "impegnata" per uno o più ordini cliente e sottoposta a un processo di controllo qualità strutturato in step tracciabili.

---

## 2. Contesto

Il cliente Gabrielli utilizza già **Intesi-FACTORY** come ERP. I dati anagrafici (clienti, commesse, righe ordine, materiali, lotti di rintracciabilità) vivono lì. Questo applicativo li legge ma non li modifica: scrive solo sul proprio database `GabrielliWF`.

---

## 3. Attori

| Attore | Descrizione |
|---|---|
| Operatore qualità | Crea impegni, definisce workflow, registra avanzamento step |
| Responsabile | Monitora lo stato di tutti gli impegni dal dashboard |

---

## 4. Funzionalità

### 4.1 Impegno Lamiera

**Come** operatore qualità  
**Voglio** impegnare una lamiera per uno o più ordini  
**Per** tracciarla nel processo qualità

**Criteri di accettazione:**
- Seleziono la lamiera da un autocomplete che cerca su `L_MLPR + S_CRN` (per lotto, codice, descrizione) — mostra solo lotti con disponibilità > 0
- Inserisco la quantità da impegnare
- Aggiungo uno o più riferimenti **Cliente → Commessa → Riga** (cascade filter: commesse filtrate per cliente, righe filtrate per commessa)
- Ogni riferimento ha una propria quantità
- Posso aggiungere una nota libera all'impegno
- L'impegno viene salvato con stato "Aperto"

### 4.2 Definizione Workflow

**Come** operatore qualità  
**Voglio** definire il workflow di qualità contestualmente alla creazione dell'impegno  
**Per** formalizzare il piano di controllo

**Criteri di accettazione:**
- Nella stessa pagina di creazione impegno, posso aggiungere gli step del workflow
- Gli step si scelgono da un catalogo (`WF_StepDef`) oppure si creano al momento (il nuovo step viene aggiunto automaticamente al catalogo)
- Ogni step può avere un **attributo di rintracciabilità** associato (da `X_JDE_AttributiAnagrafica`, es. "Prove SEP", "Ultra Suoni") con possibilità di scegliere un valore tra quelli tabellari (`X_JDE_AttributiValori`)
- Gli step sono ordinabili (su/giù) e cancellabili prima del salvataggio
- Posso caricare un **template salvato** per pre-popolare gli step
- Posso salvare il workflow appena definito come **template** (con nome) per riutilizzarlo in futuro

### 4.3 Registrazione Avanzamento Step

**Come** operatore qualità  
**Voglio** registrare l'avanzamento di ogni step del workflow  
**Per** documentare i controlli eseguiti

**Criteri di accettazione:**
- Dal dettaglio impegno, clicco su uno step per registrare il progresso
- Seleziono lo **stato** (obbligatorio): **Eseguito**, **Non OK**, **Annullato**
- Facoltativamente inserisco:
  - Valore attributo (da lista tabellare se lo step ha attributo associato)
  - Valore libero (testo)
  - Nota
  - Data prossimo step
- Quando tutti gli step sono in stato != Aperto, il workflow passa automaticamente a "Completato"
- Posso **aggiungere nuovi step** a un workflow già in corso

### 4.4 Monitoraggio Impegni (Dashboard)

**Come** responsabile  
**Voglio** vedere tutti gli impegni con il loro stato di avanzamento  
**Per** sapere a che punto è ogni lamiera nel processo qualità

**Criteri di accettazione:**
- Lista di tutti gli impegni con:
  - Lotto lamiera, codice articolo, descrizione
  - Quantità, data, stato
  - **Catena di blocchi** colorati che rappresentano gli step del workflow
- Colori dei blocchi:
  - **Verde**: step eseguito con successo
  - **Rosso**: step Non OK
  - **Arancio**: prossimo step da eseguire (primo in stato Aperto)
  - **Grigio**: step futuri
  - **Grigio scuro**: step annullati
- Hovering su un blocco mostra tooltip con: valore registrato, nota, data esecuzione, data prossimo step
- Clic su un blocco naviga al dettaglio impegno per registrare l'avanzamento
- Filtri rapidi: Tutti / Aperti / Completati / Annullati
- Ricerca per lotto/codice/descrizione

### 4.5 Gestione Template

**Come** operatore qualità  
**Voglio** gestire i template di workflow salvati  
**Per** riutilizzarli e mantenerli aggiornati

**Criteri di accettazione:**
- Lista di tutti i template con step, data creazione
- Possibilità di archiviare un template (non appare più nell'elenco di selezione)

---

## 5. Fuori scope (v1.0)

- Autenticazione/autorizzazione (utenti e ruoli)
- Modifica di un impegno già creato
- Storico modifiche degli step
- Notifiche o alert su step scaduti
- Export PDF/Excel del workflow
- Integrazione write-back su Intesi-FACTORY

---

## 6. Schema dati (DB GabrielliWF)

```
WF_Impegno (ID, IDCRN, Quantita, DataImpegno, Note, CreatoDa, Stato)
  └── WF_ImpegnoRef (ID, IDImpegno, IDCLI, CONUM, IDROW, Quantita)
  └── WF_Workflow (ID, IDImpegno, IDTemplate?, Nome, DataCreazione, CreatoDa, Stato)
       └── WF_Step (ID, IDWorkflow, Ordine, IDStepDef?, NomeCustom, IDAttributo?,
                    IDAttributoValore?, Stato, ValoreRegistrato, Nota,
                    DataEsecuzione, DataProssimoStep, RegistratoDa, DataRegistrazione)

WF_StepDef (ID, Nome, Descrizione, IsObsolete)
WF_Template (ID, Nome, Descrizione, DataCreazione, CreatoDa, IsObsolete)
  └── WF_TemplateStep (ID, IDTemplate, Ordine, IDStepDef?, NomeCustom, IDAttributo?)
```

---

## 7. Reference DB Intesi-FACTORY (read-only)

| Tabella | Uso |
|---|---|
| `S_CRN` | Lotti rintracciabilità lamiere (CRCOD = codice lotto) |
| `L_MLPR` | Disponibilità per lotto (qtà disponibile, magazzino, ubicazione) |
| `A_PAR` | Anagrafica articoli (codice, descrizione, qualità JDE) |
| `JDEQuality` | Specifiche qualità articolo |
| `A_CLI` | Anagrafica clienti |
| `A_COM` | Commesse cliente |
| `L_CMPA` | Righe ordine (articolo, quantità, unità misura) |
| `X_JDE_AttributiAnagrafica` | Tipi attributo di rintracciabilità (Alpha1-Alpha34) |
| `X_JDE_AttributiValori` | Valori tabellari per ogni tipo attributo |

---

## 8. Pagine principali

| URL | Pagina | Descrizione |
|---|---|---|
| `/` | Monitoraggio | Dashboard impegni con catena step |
| `/impegni/nuovo` | Nuovo Impegno | Crea impegno + definisci workflow |
| `/impegni/{id}` | Dettaglio Impegno | Gestione step + registrazione avanzamento |
| `/templates` | Template | Lista e archiviazione template |

---

## 9. Non-Functional Requirements

- Applicazione **intranet**, nessun requisito di sicurezza avanzata v1.0
- Deve girare su **Windows Server / IIS** o con `dotnet run` in locale
- Tempo di risposta accettabile con ~500 impegni attivi in lista
- UI responsive (desktop e tablet)
