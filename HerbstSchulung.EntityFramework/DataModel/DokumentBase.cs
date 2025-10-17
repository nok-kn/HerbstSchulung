using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HerbstSchulung.EntityFramework.DataModel;

/// <summary>
/// Abstrakte Basisklasse für Dokumente. Enthält eine Liste von Deskriptoren.
/// </summary>
public abstract class DokumentBase : EntityBase
{
    /// <summary>
    /// Menschlich lesbare Dokumentnummer.
    /// </summary>
    [Required]
    [MaxLength(50)]
    [Column("DokumentNummer")] // Beispiel: Spaltenname anpassen
    public string Nummer { get; set; } = default!;

    /// <summary>
    /// Datum des Dokuments.
    /// </summary>
    [DataType(DataType.Date)]
    public DateOnly Datum { get; set; } = DateOnly.FromDateTime(DateTime.UtcNow);

    /// <summary>
    /// Navigation zu den Deskriptoren. Verwendung einer separaten Join-Tabelle (n:n) möglich,
    /// hier demonstrativ 1:n Beziehung Dokument -> Deskriptor.
    /// </summary>
    public ICollection<Deskriptor> Deskriptoren { get; set; } = new List<Deskriptor>();
}
