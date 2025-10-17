using System.ComponentModel.DataAnnotations;

namespace HerbstSchulung.EntityFramework.DataModel;




/// <summary>
/// Basisklasse für alle Entities mit string-Id.
/// </summary>
public abstract class EntityBase
{
    protected EntityBase()
    {
        // Id nicht bei DB generieren lassen, sondern hier im Konstruktor:
        // + vermeidet DB-Roundtrips nur zur Id Erzeugung 
        // + bessere Traceability im gesamten System - IDs sind global eindeutig
        // + für verteilte, skalierbare, oder Offline Systeme sinnvoll
        // - schlecht lesbar für Menschen

        // Solche IDs sollen aber nicht erratbar sein  (ausreichend randomisiert)
        // z.B Id = Guid.NewGuid().ToString("N"); // Standard: reine GUID ohne Trennzeichen

        // Oder Ids mit Prefix:
        // + Lesbarkeit: Menschen erkennen sofort, worum es geht(z.B.Rechnungsnummern, Bestellungen).
         
        Id = SemiSemanticIdGenerator.GenerateFor(this);
    }

    
    /// <summary>
    /// Primärschlüssel. Wird von EF Core automatisch als Key erkannt aufgrund des "Id" Namens,
    /// wird hier aber zur Klarheit explizit dekoriert.
    /// </summary>
    [Key]
    [MaxLength(64)] // Längenbegrenzung in DB
    // [StringLength(64)] geht auch, aber MaxLength ist üblicher für DB-Spalten
    [Required]
    public required string Id { get; set; } 

    /// <summary>
    /// Optionale technische Erstellungszeit.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
