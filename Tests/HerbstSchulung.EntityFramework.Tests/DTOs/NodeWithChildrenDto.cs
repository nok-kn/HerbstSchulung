namespace HerbstSchulung.EntityFramework.Tests.DTOs;

/// <summary>
/// DTO f�r Node Entity mit Children - demonstriert hierarchische Struktur.
/// </summary>
public record NodeWithChildrenDto(
    string Id,
    string Name,
    string? ParentId,
    List<NodeWithChildrenDto> Children
);
