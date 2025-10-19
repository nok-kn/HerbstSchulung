using EntityFrameworkCore.Triggered;
using HerbstSchulung.EntityFramework.DataModel;
using Microsoft.Extensions.Logging;

namespace HerbstSchulung.EntityFramework.Triggers;

/// <summary>
/// Ein einfacher AfterSave-Trigger, der nach dem Speichern von EntityBase-Objekten ausgelöst wird.
/// Dieser Trigger wird automatisch nach jedem SaveChanges für alle Entities aufgerufen, die von EntityBase erben.
/// </summary>
public class EntitySavedTrigger : IAfterSaveTrigger<EntityBase>
{
    private readonly ILogger<EntitySavedTrigger>? _logger;

    public EntitySavedTrigger(ILogger<EntitySavedTrigger>? logger = null)
    {
        _logger = logger;
    }

    public Task AfterSave(ITriggerContext<EntityBase> context, CancellationToken cancellationToken)
    {
        if (_logger == null)
        {
            return Task.CompletedTask;
        }
        var entity = context.Entity;
        var entityType = entity.GetType().Name;

        switch (context.ChangeType)
        {
            case ChangeType.Added:
                _logger.LogInformation("Entity {EntityType} mit Id {Id} wurde erstellt", entityType, entity.Id);
                break;
            case ChangeType.Modified:
                _logger.LogInformation("Entity {EntityType} mit Id {Id} wurde aktualisiert", entityType, entity.Id);
                break;
            case ChangeType.Deleted:
                _logger.LogInformation("Entity {EntityType} mit Id {Id} wurde gelöscht", entityType, entity.Id);
                break;
        }

        return Task.CompletedTask;
    }
}
