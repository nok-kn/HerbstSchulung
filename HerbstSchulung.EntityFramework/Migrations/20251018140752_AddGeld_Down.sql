-- Migration: AddGeld (Rollback)
-- Beschreibung: Entfernt BetragBrutto (Geld Value Object) von Angeboten und Rechnungen

PRINT 'Starte Migration Rollback: AddGeld (Down)';
GO

-- Prüfen und Entfernen der Spalten aus der Tabelle Rechnungen
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Rechnungen]') AND name = 'BetragBrutto_Waehrung')
BEGIN
    PRINT 'Entferne Spalte [BetragBrutto_Waehrung] aus Tabelle [Rechnungen]...';
    
    ALTER TABLE [dbo].[Rechnungen]
    DROP COLUMN [BetragBrutto_Waehrung];
    
    PRINT 'Spalte [BetragBrutto_Waehrung] erfolgreich entfernt.';
END
ELSE
BEGIN
    PRINT 'Spalte [BetragBrutto_Waehrung] existiert nicht in [Rechnungen].';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Rechnungen]') AND name = 'BetragBrutto_Wert')
BEGIN
    PRINT 'Entferne Spalte [BetragBrutto_Wert] aus Tabelle [Rechnungen]...';
    
    ALTER TABLE [dbo].[Rechnungen]
    DROP COLUMN [BetragBrutto_Wert];
    
    PRINT 'Spalte [BetragBrutto_Wert] erfolgreich entfernt.';
END
ELSE
BEGIN
    PRINT 'Spalte [BetragBrutto_Wert] existiert nicht in [Rechnungen].';
END
GO

-- Prüfen und Entfernen der Spalten aus der Tabelle Angebote
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Angebote]') AND name = 'BetragBrutto_Waehrung')
BEGIN
    PRINT 'Entferne Spalte [BetragBrutto_Waehrung] aus Tabelle [Angebote]...';
    
    ALTER TABLE [dbo].[Angebote]
    DROP COLUMN [BetragBrutto_Waehrung];
    
    PRINT 'Spalte [BetragBrutto_Waehrung] erfolgreich entfernt.';
END
ELSE
BEGIN
    PRINT 'Spalte [BetragBrutto_Waehrung] existiert nicht in [Angebote].';
END
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Angebote]') AND name = 'BetragBrutto_Wert')
BEGIN
    PRINT 'Entferne Spalte [BetragBrutto_Wert] aus Tabelle [Angebote]...';
    
    ALTER TABLE [dbo].[Angebote]
    DROP COLUMN [BetragBrutto_Wert];
    
    PRINT 'Spalte [BetragBrutto_Wert] erfolgreich entfernt.';
END
ELSE
BEGIN
    PRINT 'Spalte [BetragBrutto_Wert] existiert nicht in [Angebote].';
END
GO

PRINT 'Migration Rollback AddGeld (Down) erfolgreich abgeschlossen.';
GO
