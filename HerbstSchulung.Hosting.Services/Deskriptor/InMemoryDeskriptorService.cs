using System.Collections.Concurrent;
using HerbstSchulung.Hosting.Abstractions.Deskriptor;
using Microsoft.Extensions.Logging;

namespace HerbstSchulung.Hosting.Services.Deskriptor;

/// <summary>
/// Einfache In-Memory-Implementierung von <see cref="IDeskriptorService"/>
/// Thread-sicher durch Verwendung von ConcurrentDictionary.
/// </summary>
internal sealed class InMemoryDeskriptorService(ILogger<InMemoryDeskriptorService> logger) : IDeskriptorService
{
    private readonly ConcurrentDictionary<int, DeskriptorDto> _store = new();
    private int _nextId = 1;

    public Task<DeskriptorDto> CreateAsync(DeskriptorDto dto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var id = dto.Id > 0 ? dto.Id : Interlocked.Increment(ref _nextId);
        var created = dto with { Id = id };
        _store[id] = created;
        logger.LogInformation("Deskriptor erstellt: {Id} - {Name}", created.Id, created.Name);
        return Task.FromResult(created);
    }

    public Task<DeskriptorDto?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        _store.TryGetValue(id, out var result);
        return Task.FromResult(result);
    }

    public Task<IReadOnlyList<DeskriptorDto>> GetAllAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        IReadOnlyList<DeskriptorDto> list = _store.Values.OrderBy(d => d.Id).ToList();
        return Task.FromResult(list);
    }

    public Task<bool> UpdateAsync(DeskriptorDto dto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (dto.Id <= 0) return Task.FromResult(false);

        return Task.FromResult(_store.TryGetValue(dto.Id, out var existing)
            && _store.TryUpdate(dto.Id, dto, existing));
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_store.TryRemove(id, out _));
    }
}
