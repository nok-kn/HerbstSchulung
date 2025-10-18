using System.ComponentModel.DataAnnotations;

namespace HerbstSchulung.EntityFramework.DataModel;

// Beispiel für TPC (Table-per-Concrete Type) Vererbung in EF Core
// Bei TPC erhält jeder konkrete Typ eine eigene Tabelle, die ALLE
// Eigenschaften (inkl. der geerbten) als Spalten enthält.

/// <summary>
/// Abstraktes Dokument als Basisklasse 
/// </summary>
public abstract class Dokument : EntityBase
{
    [Required]
    [MaxLength(200)]
    public required string Titel { get; set; }

    /// <summary>
    /// Optionaler Nettobetrag
    /// </summary>
    [Range(0, 1_000_000)]
    public decimal? BetragNetto { get; set; }

    /// <summary>
    /// Bruttobetrag als Value Object mit Währung
    /// </summary>
    public Geld BetragBrutto { get; set; } = Geld.ZeroEuro;
}

public class Rechnung : Dokument
{
    [Required]
    [MaxLength(32)]
    public required string Rechnungsnummer { get; set; }

    /// <summary>
    /// Zahlungsziel in Tagen
    /// </summary>
    [Range(0, 365)]
    public int ZahlungszielTage { get; set; }
}

public class Angebot : Dokument
{
    /// <summary>
    /// Bis wann ist das Angebot gültig (UTC)?
    /// </summary>
    public DateTime? GueltigBisUtc { get; set; }

    /// <summary>
    /// Optionaler Rabatt in Prozent (0..100).
    /// </summary>
    [Range(0, 100)]
    public double? RabattProzent { get; set; }
}


