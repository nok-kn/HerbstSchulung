using HerbstSchulung.EntityFramework.DataModel;
using Microsoft.EntityFrameworkCore;

namespace HerbstSchulung.EntityFramework;

public static class AppDbContextExtensions
{
    /// <summary>
    /// F�gt eine Entity zur Datenbank hinzu, falls sie noch nicht existiert (basierend auf der Id).
    /// </summary>
    /// <typeparam name="TEntity">Der Entity-Typ, der von EntityBase erbt.</typeparam>
    /// <param name="context">Der Datenbankkontext.</param>
    /// <param name="entity">Die Entity, die hinzugef�gt werden soll, falls sie noch nicht existiert.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>True, wenn die Entity hinzugef�gt wurde, false, wenn sie bereits existierte.</returns>
    public static async Task<bool> AddIfNotExistsAsync<TEntity>(
        this AppDbContext context,
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : EntityBase
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        // Pr�fen, ob eine Entity mit derselben Id bereits in der Datenbank existiert
        var exists = await context.Set<TEntity>()
            .AnyAsync(e => e.Id == entity.Id, cancellationToken);

        if (!exists)
        {
            context.Set<TEntity>().Add(entity);
            return true;
        }

        return false;
    }

    /// <summary>
    /// F�gt eine Entity hinzu oder aktualisiert sie, falls sie bereits existiert (Merge-Verhalten).
    /// Pr�ft zuerst, ob die Entity im Context getrackt wird, dann ob sie in der Datenbank existiert.
    /// </summary>
    /// <typeparam name="TEntity">Der Entity-Typ, der von EntityBase erbt.</typeparam>
    /// <param name="context">Der Datenbankkontext.</param>
    /// <param name="entity">Die Entity, die hinzugef�gt oder aktualisiert werden soll.</param>
    /// <param name="cancellationToken">Cancellation Token</param>
    /// <returns>True, wenn die Entity neu hinzugef�gt wurde, false, wenn sie aktualisiert wurde.</returns>
    public static async Task<bool> MergeAsync<TEntity>(
        this AppDbContext context,
        TEntity entity,
        CancellationToken cancellationToken = default)
        where TEntity : EntityBase
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        // Zuerst pr�fen, ob die Entity bereits im Context getrackt wird
        var tracked = context.ChangeTracker.Entries<TEntity>()
            .FirstOrDefault(e => e.Entity.Id == entity.Id);

        if (tracked != null)
        {
            // Entity ist bereits getrackt - aktualisiere die Werte (Merge)
            context.Entry(tracked.Entity).CurrentValues.SetValues(entity);
            return false;
        }

        // Pr�fen, ob eine Entity mit derselben Id bereits in der Datenbank existiert
        var exists = await context.Set<TEntity>()
            .AnyAsync(e => e.Id == entity.Id, cancellationToken);

        if (!exists)
        {
            // Entity existiert nicht - neu hinzuf�gen
            context.Set<TEntity>().Add(entity);
            return true;
        }
        else
        {
            // Entity existiert in DB - aktualisieren (Merge)
            context.Set<TEntity>().Update(entity);
            return false;
        }
    }
}
