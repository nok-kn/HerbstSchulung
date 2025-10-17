using System.ComponentModel.DataAnnotations;

namespace HerbstSchulung.EntityFramework.DataModel;

/// <summary>
/// Basisklasse für alle Entities mit string-Id.
/// </summary>
public abstract class EntityBase
{
    protected EntityBase()
    {
        Id = Guid.NewGuid().ToString("N"); // Beispielhafte Initialisierung mit GUID als lesbarer String
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
