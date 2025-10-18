namespace HerbstSchulung.EntityFramework;

/// <summary>
/// Factory-Interface für das Erstellen von AppDbContext-Instanzen.
/// Ermöglicht verschiedene Implementierungen für Produktion (SQL Server) und Tests (InMemory).
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
    /// Ideal für Lesevorgänge, bei denen keine Änderungen gespeichert werden sollen.
    /// Verbessert die Performance bei reinen Leseoperationen.
    /// </summary>
    /// <returns>Eine neue AppDbContext-Instanz mit QueryTrackingBehavior.NoTracking.</returns>
    AppDbContext CreateReadOnlyContext();
}
