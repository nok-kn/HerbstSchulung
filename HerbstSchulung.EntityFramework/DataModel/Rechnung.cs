using System.ComponentModel.DataAnnotations;

namespace HerbstSchulung.EntityFramework.DataModel;

/// <summary>
/// Konkrete Dokument-Entity: Rechnung.
/// </summary>
public class Rechnung : DokumentBase
{
    /// <summary>
    /// Netto-Betrag der Rechnung.
    /// </summary>
    [Range(0, double.MaxValue)]
    public decimal BetragNetto { get; set; }

    /// <summary>
    /// Mehrwertsteuer in Prozent.
    /// </summary>
    [Range(0, 100)]
    public decimal MwstProzent { get; set; }

    /// <summary>
    /// Optionaler Kunde.
    /// </summary>
    [MaxLength(200)]
    public string? Kunde { get; set; }
}
