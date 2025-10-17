using System.ComponentModel.DataAnnotations;

namespace HerbstSchulung.EntityFramework.DataModel;

/// <summary>
/// Konkrete Implementierung eines Deskriptors.
/// </summary>
public class Deskriptor : DeskriptorBase
{
    /// <summary>
    /// Freitext-Wert des Deskriptors.
    /// </summary>
    [MaxLength(2048)]
    public string? Value { get; set; }
}
