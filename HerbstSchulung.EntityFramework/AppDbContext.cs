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

    public DbSet<Student> Students => Set<Student>();
    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Auto> Autos => Set<Auto>();
    public DbSet<Lastkraftwagen> Lastkraftwagen => Set<Lastkraftwagen>();
    public DbSet<Land> Laender => Set<Land>();

    public bool IsReadOnly { get; init; }

    /// <summary>
    /// Modellkonfiguration. Hier legen wir u. a. die Vererbungstrategie und Relationen fest.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TPH
        modelBuilder.Entity<Person>().UseTphMappingStrategy() 
            .ToTable("Persons")
            .HasDiscriminator(p => p.Art)
            .HasValue<Student>(PersonArt.Student)
            .HasValue<Teacher>(PersonArt.Teacher);

        // TPT
        modelBuilder.Entity<Fahrzeug>().UseTptMappingStrategy();
        modelBuilder.Entity<Fahrzeug>().ToTable("Fahrzeuge");
        modelBuilder.Entity<Auto>().ToTable("Autos");
        modelBuilder.Entity<Lastkraftwagen>().ToTable("Lastkraftwagen");
        modelBuilder.Entity<Land>().ToTable("Laender");
        
        // TPC
        modelBuilder.Entity<Dokument>().UseTpcMappingStrategy(); 
        modelBuilder.Entity<Rechnung>().ToTable("Rechnungen");
        modelBuilder.Entity<Angebot>().ToTable("Angebote");


        // Seed für statischen Daten
        var seedCreated = new DateTime(2025, 01, 01, 0, 0, 0, DateTimeKind.Utc);
        modelBuilder.Entity<Land>().HasData(
            new { Id = "LND-DE000001", Name = "Deutschland", IsoCode = "DE", CreatedUtc = seedCreated },
            new { Id = "LND-AT000001", Name = "Österreich", IsoCode = "AT", CreatedUtc = seedCreated },
            new { Id = "LND-CH000001", Name = "Schweiz", IsoCode = "CH", CreatedUtc = seedCreated }
        );
    }

    public override int SaveChanges()
    {
        ThrowIfReadOnly();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        ThrowIfReadOnly();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfReadOnly();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        ThrowIfReadOnly();
        return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void ThrowIfReadOnly()
    {
        if (IsReadOnly)
        {
            throw new InvalidOperationException("This context is read-only. Save operations are not allowed.");
        }
    }


}
