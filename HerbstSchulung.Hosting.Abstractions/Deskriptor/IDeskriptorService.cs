namespace HerbstSchulung.Hosting.Abstractions.Deskriptor;

/// <summary>
/// Service-Schnittstelle Operationen auf <see cref="DeskriptorDto"/>.
/// </summary>
public interface IDeskriptorService
{
    /// <summary>
    /// Legt einen neuen Deskriptor an. Falls <paramref name="dto"/> keine Id besitzt (<= 0),
    /// wird automatisch eine neue Id vergeben.
    /// </summary>
    Task<DeskriptorDto> CreateAsync(DeskriptorDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Liefert einen Deskriptor per Id oder <c>null</c>, wenn nicht vorhanden.
    /// </summary>
    Task<DeskriptorDto?> GetByIdAsync(int id, CancellationToken cancellationToken);

    /// <summary>
    /// Liefert alle vorhandenen Deskriptoren.
    /// </summary>
    Task<IReadOnlyList<DeskriptorDto>> GetAllAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Aktualisiert einen bestehenden Deskriptor. Gibt <c>true</c> zurck, falls aktualisiert,
    /// andernfalls <c>false</c> (nicht gefunden).
    /// </summary>
    Task<bool> UpdateAsync(DeskriptorDto dto, CancellationToken cancellationToken);

    /// <summary>
    /// Löscht einen Deskriptor. Gibt <c>true</c> zurück, falls gelöscht, andernfalls <c>false</c>.
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
