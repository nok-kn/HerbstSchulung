-- Migration: AddGeld
-- Beschreibung: Fügt BetragBrutto (Geld Value Object) zu Angeboten und Rechnungen hinzu

PRINT 'Starte Migration: AddGeld (Up)';
GO

-- Prüfen und Hinzufügen der Spalten zur Tabelle Rechnungen
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Rechnungen]') AND name = 'BetragBrutto_Wert')
BEGIN
    PRINT 'Füge Spalte [BetragBrutto_Wert] zur Tabelle [Rechnungen] hinzu...';
    
    ALTER TABLE [dbo].[Rechnungen]
    ADD [BetragBrutto_Wert] decimal(18,2) NULL;
    
    PRINT 'Spalte [BetragBrutto_Wert] erfolgreich hinzugefügt.';
END
ELSE
BEGIN
    PRINT 'Spalte [BetragBrutto_Wert] existiert bereits in [Rechnungen].';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Rechnungen]') AND name = 'BetragBrutto_Waehrung')
BEGIN
    PRINT 'Füge Spalte [BetragBrutto_Waehrung] zur Tabelle [Rechnungen] hinzu...';
    
    ALTER TABLE [dbo].[Rechnungen]
    ADD [BetragBrutto_Waehrung] nvarchar(3) NULL;
    
    PRINT 'Spalte [BetragBrutto_Waehrung] erfolgreich hinzugefügt.';
END
ELSE
BEGIN
    PRINT 'Spalte [BetragBrutto_Waehrung] existiert bereits in [Rechnungen].';
END
GO

-- Prüfen und Hinzufügen der Spalten zur Tabelle Angebote
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Angebote]') AND name = 'BetragBrutto_Wert')
BEGIN
    PRINT 'Füge Spalte [BetragBrutto_Wert] zur Tabelle [Angebote] hinzu...';
    
    ALTER TABLE [dbo].[Angebote]
    ADD [BetragBrutto_Wert] decimal(18,2) NULL;
    
    PRINT 'Spalte [BetragBrutto_Wert] erfolgreich hinzugefügt.';
END
ELSE
BEGIN
    PRINT 'Spalte [BetragBrutto_Wert] existiert bereits in [Angebote].';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Angebote]') AND name = 'BetragBrutto_Waehrung')
BEGIN
    PRINT 'Füge Spalte [BetragBrutto_Waehrung] zur Tabelle [Angebote] hinzu...';
    
    ALTER TABLE [dbo].[Angebote]
    ADD [BetragBrutto_Waehrung] nvarchar(3) NULL;
    
    PRINT 'Spalte [BetragBrutto_Waehrung] erfolgreich hinzugefügt.';
END
ELSE
BEGIN
    PRINT 'Spalte [BetragBrutto_Waehrung] existiert bereits in [Angebote].';
END
GO

PRINT 'Migration AddGeld (Up) erfolgreich abgeschlossen.';
GO
