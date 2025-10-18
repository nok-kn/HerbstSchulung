-- Migration: AddDokumentEntities (Rollback)
-- Beschreibung: Entfernt die Tabellen für Dokumente (Angebote und Rechnungen)

PRINT 'Starte Migration Rollback: AddDokumentEntities (Down)';
GO

-- Prüfen und Löschen der Tabelle Angebote
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Angebote' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    PRINT 'Lösche Tabelle [Angebote]...';
    
    DROP TABLE [dbo].[Angebote];
    
    PRINT 'Tabelle [Angebote] erfolgreich gelöscht.';
END
ELSE
BEGIN
    PRINT 'Tabelle [Angebote] existiert nicht.';
END
GO

-- Prüfen und Löschen der Tabelle Rechnungen
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Rechnungen' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    PRINT 'Lösche Tabelle [Rechnungen]...';
    
    DROP TABLE [dbo].[Rechnungen];
    
    PRINT 'Tabelle [Rechnungen] erfolgreich gelöscht.';
END
ELSE
BEGIN
    PRINT 'Tabelle [Rechnungen] existiert nicht.';
END
GO

PRINT 'Migration Rollback AddDokumentEntities (Down) erfolgreich abgeschlossen.';
GO
