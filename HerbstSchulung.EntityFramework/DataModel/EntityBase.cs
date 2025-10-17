using System.ComponentModel.DataAnnotations;

namespace HerbstSchulung.EntityFramework.DataModel;

/// <summary>
/// Basisklasse für alle Entities mit string-Id.
/// Hinweis: string-Ids eignen sich z. B. für ULIDs, GUIDs als String oder eigene Schlüssel.
/// </summary>
public abstract class EntityBase
{
    public EntityBase()
    {
        Id = Guid.NewGuid().ToString("N"); // Beispielhafte Initialisierung mit GUID als lesbarer String
    }

    /// <summary>
    /// Primärschlüssel. Wird von EF Core automatisch als Key erkannt aufgrund des "Id" Namens,
    /// wird hier aber zur Klarheit explizit dekoriert.
    /// </summary>
    [Key]
    [MaxLength(64)] // Beispielhafte Längenbegrenzung für lesbare Schlüssel (z. B. ULID)
    [Required]
    public string Id { get; set; } 

    /// <summary>
    /// Optionale technische Erstellungszeit.
    /// </summary>
    [DataType(DataType.DateTime)]
    public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
}
