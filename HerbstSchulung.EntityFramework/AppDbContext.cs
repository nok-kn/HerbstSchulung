using Microsoft.EntityFrameworkCore;
using HerbstSchulung.EntityFramework.DataModel;

namespace HerbstSchulung.EntityFramework;

/// <summary>
/// EF Core DbContext für das Beispiel. Nutzt Code First und konfigurierte Tabellen- und Spaltennamen.
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Deskriptor> Deskriptoren => Set<Deskriptor>();
    public DbSet<Rechnung> Rechnungen => Set<Rechnung>();

    /// <summary>
    /// Modellkonfiguration. Hier legen wir u. a. die Vererbungstrategie fest und konfigurieren Tabellennamen.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Generelle Konvention: Pluralisierte Tabellennamen
        modelBuilder.Entity<Deskriptor>().ToTable("Deskriptoren");

        // Vererbung für Dokumente: TPH (Table-per-Hierarchy)
        // Eine Tabelle "Dokumente" für die gesamte Hierarchie; konkrete Typen werden per Discriminator unterschieden
        modelBuilder.Entity<DokumentBase>()
            .UseTphMappingStrategy() // TPH-Strategie
            .ToTable("Dokumente")
            .HasDiscriminator<string>("DokumentTyp")
            .HasValue<Rechnung>("Rechnung");

        // Beziehungen: 1:n Dokument -> Deskriptor
        modelBuilder.Entity<DokumentBase>()
            .HasMany(d => d.Deskriptoren)
            .WithOne() // kein Backref-Eigentümer in diesem einfachen Beispiel
            .OnDelete(DeleteBehavior.Cascade);

        // Indizes und Constraints als Beispiele
        modelBuilder.Entity<EntityBase>()
            .HasIndex(e => e.Id)
            .IsUnique();

        modelBuilder.Entity<Deskriptor>()
            .HasIndex(d => new { d.Name, d.Value });

        // Property-Konfigurationen für Value Objects
        modelBuilder.Entity<DokumentBase>()
            .Property(d => d.Datum)
            .HasColumnType("date"); // Map DateOnly auf SQL-Typ date
    }
}
