using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HerbstSchulung.EntityFramework.DataModel;

/// <summary>
/// Beispiel-Entity für Länder.
/// </summary>
[Table("Laender")]
public class Land : EntityBase
{
    [Required]
    [MaxLength(100)]
    public required string Name { get; set; }

    [Required]
    [MaxLength(3)]
    public required string IsoCode { get; set; }
}

