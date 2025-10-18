namespace HerbstSchulung.EntityFramework;

/// <summary>
/// Factory-Interface f�r das Erstellen von AppDbContext-Instanzen.
/// Erm�glicht verschiedene Implementierungen f�r Produktion (SQL Server) und Tests (InMemory).
/// </summary>
public interface IAppDbContextFactory
{
    /// <summary>
    /// Erstellt einen Standard-AppDbContext mit normalem Tracking-Verhalten.
    /// </summary>
    /// <returns>Eine neue AppDbContext-Instanz.</returns>
    AppDbContext CreateContext();

    /// <summary>
    /// Erstellt einen Read-Only AppDbContext mit deaktiviertem Entity Tracking.
    /// Ideal f�r Lesevorg�nge, bei denen keine �nderungen gespeichert werden sollen.
    /// Verbessert die Performance bei reinen Leseoperationen.
    /// </summary>
    /// <returns>Eine neue AppDbContext-Instanz mit QueryTrackingBehavior.NoTracking.</returns>
    AppDbContext CreateReadOnlyContext();
}
