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
    
    /// <summary>
    /// Modellkonfiguration. Hier legen wir u. a. die Vererbungstrategie und Relationen fest.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // TPH
        modelBuilder.Entity<Person>()
            .UseTphMappingStrategy() // TPH-Strategie
            .ToTable("Persons")
            .HasDiscriminator(p => p.Art)
            .HasValue<Student>(PersonArt.Student)
            .HasValue<Teacher>(PersonArt.Teacher);

        // TPT
        modelBuilder.Entity<Fahrzeug>().UseTptMappingStrategy();
        modelBuilder.Entity<Fahrzeug>().ToTable("Fahrzeuge");
        modelBuilder.Entity<Auto>().ToTable("Autos");
        modelBuilder.Entity<Lastkraftwagen>().ToTable("Lastkraftwagen");



    }
}
