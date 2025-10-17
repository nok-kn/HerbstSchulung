# Entity Framework Core – Basis & Advanced


## Grundbegriffe
- DbContext: Unit of Work + Change Tracker
- DbSet<TEntity>: Tabelle/Collection einer Entity
- Entity: Domain-Objekt mit Identität, das persistiert wird
- Value Object: Werttyp ohne Identität (z. B. DateOnly, Money)

## Projektstruktur (Workshop)
- DataModel: Entities und Konfiguration
- AppDbContext: zentrale EF-Konfiguration
- ServiceCollectionExtensions: DI-Registrierung
- Migrations: per CLI erzeugt und verwaltet

## Code First und Inheritance
- Code First: Modell wird aus C#-Klassen abgeleitet

| Strategie                         | Beschreibung                                                                              | Vorteile                                                                                                 | Nachteile                                                                                                                       |
| --------------------------------- | ----------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------- |
| **TPH** (Table-per-Hierarchy)     | Eine Tabelle für die gesamte Vererbungshierarchie. Discriminator-Spalte bestimmt den Typ. Default.  | - Beste Performance bei einfachen Abfragen<br>- Geringerer Speicherbedarf<br>- Einfach zu implementieren | - Viele NULL-Werte bei nicht geteilten Properties<br>- Schwer wartbar bei großen Hierarchien                                    |
| **TPT** (Table-per-Type)          | Eine Tabelle pro Klasse. Vererbung wird durch Joins zwischen Tabellen abgebildet.         | - Gute Normalisierung<br>- Keine NULL-Spalten<br>- Klarere Datenstruktur                                 | - Performance schlechter durch JOINs<br>- Komplexere Abfragen<br>- Schlechtere Skalierbarkeit                                   |
| **TPC** (Table-per-Concrete-Type) | Jede konkrete Klasse hat ihre eigene Tabelle mit allen Feldern (auch von Basisklasse).    | - Beste Performance bei reinen konkreten Typen<br>- Keine NULL-Werte<br>- Kein Join nötig                | - Datenredundanz (gleiche Felder in mehreren Tabellen)<br>-                                                                     |

- Empfehlungen:
  - Lange Kette von Vererbungen vermeiden
  - Beachte, dass Strategien nicht gemischt werden können
  - Eine abstrakte Basisklasse (Id, CreatedAt, etc.) für alle Entities ist sinnvoll
  - Benutze folgenden Entscheidungsbaum zur Auswahl der Strategie: 


Start
│
└── Gibt es sehr hohe Anforderungen an Abfrage Performance?
      │
      ├── Ja
      │     └── Ist Datenredundanz akzeptabel?
      │           ├── Ja   => Verwende **TPC**
      │           └── Nein => Verwende **TPH**
      │
      └── Nein
            └── Hast du viele gemeinsame Properties in der Basisklasse?
                  │
                  ├── Ja
                  │     └── Möchtest du NULL-Werte vermeiden?
                  │           ├── Ja   => Verwende **TPT**
                  │           └── Nein => Verwende **TPH**
                  │
                  └── Nein
                        └── Sind die Typen stark unterschiedlich?
                              ├── Ja   => Verwende **TPC**
                              └── Nein => Verwende **TPH**




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
- Erzeugung: `dotnet ef migrations add InitialCreate -p HerbstSchulung.EntityFramework`
- Aktualisieren: `dotnet ef database update -p HerbstSchulung.EntityFramework`
- Entfernen: `dotnet ef migrations remove -p HerbstSchulung.EntityFramework`

Hinweise:
- -p = Projekt mit DbContext, -s = Startprojekt (liefert Provider/Config)
- Für reine Bibliotheken: Design-Time Factory implementieren (IDesignTimeDbContextFactory)

## Best Practices
- Explizite Konfigurationen für Keys, Längen, Indizes
- Keine Business-Logik im DbContext
- Transaktionen bewusst einsetzen (`BeginTransactionAsync`)
- AsNoTracking für reine Lesezugriffe
- Owned Types für Value Objects

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
