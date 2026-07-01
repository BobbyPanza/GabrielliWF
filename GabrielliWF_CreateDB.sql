-- =============================================================
-- GabrielliWF — Script creazione database
-- Eseguire con un utente che abbia permessi CREATE DATABASE
-- oppure solo la parte "USE GabrielliWF" se il DB è già stato
-- creato dal DBA e l'utente applicativo ha db_owner su di esso.
-- =============================================================

-- -------------------------------------------------------------
-- 1. Creazione database (eseguire solo se si ha CREATE DATABASE)
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'GabrielliWF')
BEGIN
    CREATE DATABASE [GabrielliWF]
        COLLATE Latin1_General_CI_AS;
    PRINT 'Database GabrielliWF creato.';
END
GO

USE [GabrielliWF];
GO

-- -------------------------------------------------------------
-- 2. Catalogo step (senza FK, tabella base)
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'WF_StepDef')
BEGIN
    CREATE TABLE [dbo].[WF_StepDef] (
        [ID]            INT            IDENTITY(1,1) NOT NULL,
        [Nome]          NVARCHAR(200)  NOT NULL,
        [Descrizione]   NVARCHAR(500)  NULL,
        [IsObsolete]    BIT            NOT NULL DEFAULT 0,
        [DataCreazione] DATETIME2      NOT NULL DEFAULT GETDATE(),
        CONSTRAINT [PK_WF_StepDef] PRIMARY KEY ([ID])
    );
    PRINT 'Tabella WF_StepDef creata.';
END
GO

-- -------------------------------------------------------------
-- 3. Template workflow
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'WF_Template')
BEGIN
    CREATE TABLE [dbo].[WF_Template] (
        [ID]            INT            IDENTITY(1,1) NOT NULL,
        [Nome]          NVARCHAR(200)  NOT NULL,
        [Descrizione]   NVARCHAR(500)  NULL,
        [DataCreazione] DATETIME2      NOT NULL DEFAULT GETDATE(),
        [CreatoDa]      NVARCHAR(100)  NULL,
        [IsObsolete]    BIT            NOT NULL DEFAULT 0,
        CONSTRAINT [PK_WF_Template] PRIMARY KEY ([ID])
    );
    PRINT 'Tabella WF_Template creata.';
END
GO

-- -------------------------------------------------------------
-- 4. Step di un template
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'WF_TemplateStep')
BEGIN
    CREATE TABLE [dbo].[WF_TemplateStep] (
        [ID]           INT            IDENTITY(1,1) NOT NULL,
        [IDTemplate]   INT            NOT NULL,
        [Ordine]       INT            NOT NULL,
        [IDStepDef]    INT            NULL,
        [NomeCustom]   NVARCHAR(200)  NULL,
        [IDAttributo]  INT            NULL,
        [NoteTemplate] NVARCHAR(500)  NULL,
        CONSTRAINT [PK_WF_TemplateStep] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_WF_TemplateStep_Template]
            FOREIGN KEY ([IDTemplate]) REFERENCES [WF_Template]([ID]),
        CONSTRAINT [FK_WF_TemplateStep_StepDef]
            FOREIGN KEY ([IDStepDef])  REFERENCES [WF_StepDef]([ID])
    );
    PRINT 'Tabella WF_TemplateStep creata.';
END
GO

-- -------------------------------------------------------------
-- 5. Impegno lamiera
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'WF_Impegno')
BEGIN
    CREATE TABLE [dbo].[WF_Impegno] (
        [ID]            INT             IDENTITY(1,1) NOT NULL,
        [IDCRN]         INT             NOT NULL,
        [Quantita]      DECIMAL(18,4)   NOT NULL DEFAULT 0,
        [DataImpegno]   DATETIME2       NOT NULL DEFAULT GETDATE(),
        [Note]          NVARCHAR(MAX)   NULL,
        [CreatoDa]      NVARCHAR(100)   NULL,
        [Stato]         SMALLINT        NOT NULL DEFAULT 0,
        [JdeQuality]    NVARCHAR(50)    NULL,
        [QualityDesc]   NVARCHAR(200)   NULL,
        CONSTRAINT [PK_WF_Impegno] PRIMARY KEY ([ID])
    );
    PRINT 'Tabella WF_Impegno creata.';
END
GO

-- -------------------------------------------------------------
-- 6. Riferimenti cliente/commessa/riga per impegno
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'WF_ImpegnoRef')
BEGIN
    CREATE TABLE [dbo].[WF_ImpegnoRef] (
        [ID]         INT            IDENTITY(1,1) NOT NULL,
        [IDImpegno]  INT            NOT NULL,
        [IDCLI]      INT            NOT NULL,
        [CONUM]      INT            NOT NULL,
        [IDROW]      INT            NOT NULL,
        [Quantita]   DECIMAL(18,4)  NOT NULL DEFAULT 0,
        [CtRag]      NVARCHAR(200)  NULL,
        [CoCod]      NVARCHAR(100)  NULL,
        CONSTRAINT [PK_WF_ImpegnoRef] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_WF_ImpegnoRef_Impegno]
            FOREIGN KEY ([IDImpegno]) REFERENCES [WF_Impegno]([ID])
    );
    PRINT 'Tabella WF_ImpegnoRef creata.';
END
GO

-- -------------------------------------------------------------
-- 7. Workflow associato a un impegno
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'WF_Workflow')
BEGIN
    CREATE TABLE [dbo].[WF_Workflow] (
        [ID]            INT            IDENTITY(1,1) NOT NULL,
        [IDImpegno]     INT            NOT NULL,
        [IDTemplate]    INT            NULL,
        [Nome]          NVARCHAR(200)  NULL,
        [DataCreazione] DATETIME2      NOT NULL DEFAULT GETDATE(),
        [CreatoDa]      NVARCHAR(100)  NULL,
        [Stato]         SMALLINT       NOT NULL DEFAULT 0,
        CONSTRAINT [PK_WF_Workflow] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_WF_Workflow_Impegno]
            FOREIGN KEY ([IDImpegno])  REFERENCES [WF_Impegno]([ID]),
        CONSTRAINT [FK_WF_Workflow_Template]
            FOREIGN KEY ([IDTemplate]) REFERENCES [WF_Template]([ID])
    );
    PRINT 'Tabella WF_Workflow creata.';
END
GO

-- -------------------------------------------------------------
-- 8. Step di un workflow (con avanzamento)
-- -------------------------------------------------------------
IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'WF_Step')
BEGIN
    CREATE TABLE [dbo].[WF_Step] (
        [ID]                  INT            IDENTITY(1,1) NOT NULL,
        [IDWorkflow]          INT            NOT NULL,
        [Ordine]              INT            NOT NULL,
        [IDStepDef]           INT            NULL,
        [NomeCustom]          NVARCHAR(200)  NULL,
        [IDAttributo]         INT            NULL,
        [IDAttributoValore]   INT            NULL,
        [Stato]               SMALLINT       NOT NULL DEFAULT 0,
        [ValoreRegistrato]    NVARCHAR(500)  NULL,
        [Nota]                NVARCHAR(MAX)  NULL,
        [DataEsecuzione]      DATETIME2      NULL,
        [DataProssimoStep]    DATE           NULL,
        [RegistratoDa]        NVARCHAR(100)  NULL,
        [DataRegistrazione]   DATETIME2      NULL,
        CONSTRAINT [PK_WF_Step] PRIMARY KEY ([ID]),
        CONSTRAINT [FK_WF_Step_Workflow]
            FOREIGN KEY ([IDWorkflow]) REFERENCES [WF_Workflow]([ID]),
        CONSTRAINT [FK_WF_Step_StepDef]
            FOREIGN KEY ([IDStepDef])  REFERENCES [WF_StepDef]([ID])
    );
    PRINT 'Tabella WF_Step creata.';
END
GO

-- -------------------------------------------------------------
-- Fine script
-- Valori Stato (SMALLINT):
--   StatoImpegno:  0=Aperto  1=Completato  2=Annullato
--   StatoWorkflow: 0=Aperto  1=Completato  2=Annullato
--   StatoStep:     0=Aperto  1=Eseguito    2=NonOk  3=Annullato
-- -------------------------------------------------------------
PRINT 'Setup GabrielliWF completato.';
GO
