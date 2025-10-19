using System.ComponentModel.DataAnnotations;

namespace HerbstSchulung.EntityFramework.DataModel;

/// <summary>
/// Beispiel für einen Hierarchie (Parent-Child Beziehung).
/// Ein Node kann einen Parent haben und mehrere Children.
/// </summary>
public class Node : EntityBase
{
    [Required]
    [MaxLength(200)]
    public required string Name { get; set; }

    /// <summary>
    /// Foreign Key zum Parent-Node (nullable, weil Root-Nodes keinen Parent haben).
    /// </summary>
    [MaxLength(64)]
    public string? ParentId { get; set; }

    /// <summary>
    /// Navigation Property zum Parent-Node.
    /// </summary>
    public Node? Parent { get; set; }

    /// <summary>
    /// Navigation Property zu den Child-Nodes.
    /// </summary>
    public ICollection<Node> Children { get; set; } = new List<Node>();
}
