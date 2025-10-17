using System.ComponentModel.DataAnnotations;

namespace HerbstSchulung.EntityFramework.DataModel;

/// <summary>
/// Basisklasse für alle Deskriptor-Entities. Dient als TPH/ TPT-Basis je nach Strategie.
/// </summary>
public abstract class DeskriptorBase : EntityBase
{
    /// <summary>
    /// Technischer Typname (Discriminator/Abstraktion). Optional, aber nützlich für TPH.
    /// </summary>
    [MaxLength(64)]
    public string? Type { get; set; }

    /// <summary>
    /// Allgemeiner Name des Deskriptors.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = default!;
}
