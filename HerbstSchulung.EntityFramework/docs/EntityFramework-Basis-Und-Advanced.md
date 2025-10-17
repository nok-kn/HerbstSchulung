# Entity Framework Core – Basis & Advanced (Workshop)

> Alle Erklärungen in diesem Dokument sind bewusst knapp und praxisorientiert. Die Beispiele beziehen sich auf .NET 8 und EF Core 8.

## Ziele
- EF Core verstehen (Architektur, Patterns, Provider)
- Code First, Migrations, Inheritance Strategien
- Beziehungen, Indizes, Concurrency, Transaktionen
- Best Practices und häufige Fallstricke

## Grundbegriffe
- DbContext: Unit of Work + Change Tracker
- DbSet<TEntity>: Tabelle/Collection einer Entity
- Entity: Zustandsbehaftetes Domain-Objekt mit Identität
- Value Object: Werttyp ohne Identität (z. B. DateOnly, Money)

## Projektstruktur (Workshop)
- DataModel: Entities und Konfiguration
- AppDbContext: zentrale EF-Konfiguration
- ServiceCollectionExtensions: DI-Registrierung
- Migrations: per CLI erzeugt und verwaltet

## Code First und Inheritance
- Code First: Modell wird aus C#-Klassen abgeleitet
- Inheritance-Strategien:
  - TPH (Table per Hierarchy): Eine Tabelle, Discriminator-Spalte; schnell, wenig Joins
  - TPT (Table per Type): Eine Tabelle pro Typ; sauber, aber mehr Joins
  - TPC (Table per Concrete Type): Keine gemeinsame Tabelle; Duplikate möglich, Mapping aufwendig
- Empfehlung: Start mit TPH; bei klaren Gründen TPT/TPC

## Konfiguration im Beispiel
- TPH für DokumentBase -> Rechnung mit Discriminator "DokumentTyp"
- Pluralisierte Tabellennamen: Dokumente, Rechnungen, Deskriptoren
- DateOnly wird als SQL "date" gemappt
- Indizes und Constraints per Fluent API

## Beziehungen
- 1:n Dokument -> Deskriptor (Cascade Delete)
- Alternativ: n:n über Join-Tabelle mit UsingEntity

Beispiel (n:n):

```csharp
modelBuilder.Entity<DokumentBase>()
    .HasMany(d => d.Deskriptoren)
    .WithMany()
    .UsingEntity(j => j.ToTable("DokumentDeskriptoren"));
```

## Migrations
- Erzeugung: `dotnet ef migrations add InitialCreate -p HerbstSchulung.EntityFramework -s HerbstSchulung.WebApi`
- Aktualisieren: `dotnet ef database update -p HerbstSchulung.EntityFramework -s HerbstSchulung.WebApi`
- Entfernen: `dotnet ef migrations remove -p HerbstSchulung.EntityFramework -s HerbstSchulung.WebApi`

Hinweise:
- -p = Projekt mit DbContext, -s = Startprojekt (liefert Provider/Config)
- Für reine Bibliotheken: Design-Time Factory implementieren (IDesignTimeDbContextFactory)

## Best Practices
- Explizite Konfigurationen für Keys, Längen, Indizes
- Keine Business-Logik im DbContext
- Transaktionen bewusst einsetzen (`BeginTransactionAsync`)
- AsNoTracking für reine Lesezugriffe
- Owned Types für Value Objects
- Batch-Operationen vermeiden oder Tools wie EFCore.BulkExtensions nutzen

## Häufige Fallstricke
- N+1-Queries: `Include`/`ThenInclude` oder explizite Lade-Strategien verwenden
- Lazy Loading vorsichtig einsetzen (Performance/Seiteneffekte)
- Zu breite DbContext-Lebenszeit (Singleton vermeiden)
- Falsche DeleteBehavior-Einstellungen (Restrict vs. Cascade)

## Testing
- Für Unit-Tests InMemory oder SQLite In-Memory verwenden
- Für Integrations-Tests echte DB (lokal/Container) einsetzen

## Zusätzliche Ressourcen
- Offizielle Doku EF Core: https://learn.microsoft.com/ef/core/
- EF Core Konfiguration/Modeling: https://learn.microsoft.com/ef/core/modeling/
- Vererbung in EF Core: https://learn.microsoft.com/ef/core/modeling/inheritance
- Migrations: https://learn.microsoft.com/ef/core/managing-schemas/migrations/
- Performance-Tipps: https://learn.microsoft.com/ef/core/performance/
- Best Practices von Microsoft: https://learn.microsoft.com/ef/core/best-practices/
- EF Power Tools Visual Studio Ex https://marketplace.visualstudio.com/items?itemName=ErikEJ.EFCorePowerTools&ssr=false#overview
- Blog von Jeremy Likness (MS): https://blog.jeremylikness.com/
- Brice Lambson (EF Team): https://www.bricelam.net/
```
