-- Migration: AddDokumentEntities
-- Beschreibung: Erstellt die Tabellen für Dokumente (TPC-Strategie: Angebote und Rechnungen)

PRINT 'Starte Migration: AddDokumentEntities (Up)';
GO

-- Prüfen und Erstellen der Tabelle Angebote
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Angebote' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    PRINT 'Erstelle Tabelle [Angebote]...';
    
    CREATE TABLE [dbo].[Angebote] (
        [Id] nvarchar(64) NOT NULL,
        [Titel] nvarchar(200) NOT NULL,
        [BetragNetto] decimal(18,2) NULL,
        [GueltigBisUtc] datetime2 NULL,
        [RabattProzent] float NULL,
        [CreatedUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Angebote] PRIMARY KEY ([Id])
    );
    
    PRINT 'Tabelle [Angebote] erfolgreich erstellt.';
END
ELSE
BEGIN
    PRINT 'Tabelle [Angebote] existiert bereits.';
END
GO

-- Prüfen und Erstellen der Tabelle Rechnungen
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Rechnungen' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    PRINT 'Erstelle Tabelle [Rechnungen]...';
    
    CREATE TABLE [dbo].[Rechnungen] (
        [Id] nvarchar(64) NOT NULL,
        [Titel] nvarchar(200) NOT NULL,
        [BetragNetto] decimal(18,2) NULL,
        [Rechnungsnummer] nvarchar(32) NOT NULL,
        [ZahlungszielTage] int NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        CONSTRAINT [PK_Rechnungen] PRIMARY KEY ([Id])
    );
    
    PRINT 'Tabelle [Rechnungen] erfolgreich erstellt.';
END
ELSE
BEGIN
    PRINT 'Tabelle [Rechnungen] existiert bereits.';
END
GO

PRINT 'Migration AddDokumentEntities (Up) erfolgreich abgeschlossen.';
GO
