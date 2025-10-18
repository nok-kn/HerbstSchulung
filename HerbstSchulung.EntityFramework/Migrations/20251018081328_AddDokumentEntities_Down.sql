-- Migration: AddDokumentEntities (Rollback)
-- Beschreibung: Entfernt die Tabellen f�r Dokumente (Angebote und Rechnungen)

PRINT 'Starte Migration Rollback: AddDokumentEntities (Down)';
GO

-- Pr�fen und L�schen der Tabelle Angebote
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Angebote' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    PRINT 'L�sche Tabelle [Angebote]...';
    
    DROP TABLE [dbo].[Angebote];
    
    PRINT 'Tabelle [Angebote] erfolgreich gel�scht.';
END
ELSE
BEGIN
    PRINT 'Tabelle [Angebote] existiert nicht.';
END
GO

-- Pr�fen und L�schen der Tabelle Rechnungen
IF EXISTS (SELECT * FROM sys.tables WHERE name = 'Rechnungen' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    PRINT 'L�sche Tabelle [Rechnungen]...';
    
    DROP TABLE [dbo].[Rechnungen];
    
    PRINT 'Tabelle [Rechnungen] erfolgreich gel�scht.';
END
ELSE
BEGIN
    PRINT 'Tabelle [Rechnungen] existiert nicht.';
END
GO

PRINT 'Migration Rollback AddDokumentEntities (Down) erfolgreich abgeschlossen.';
GO
