# Entity Framework Core – Basis

## Grundbegriffe
- DbContext: Unit of Work + Change Tracker
- DbSet<TEntity>: Tabelle/Collection einer Entity
- Entity: Domain-Objekt mit Identität, das persistiert wird
- Value Object: Werttyp ohne Identität (z. B. DateOnly, Money)
- Code First: Modell wird aus C#-Klassen abgeleitet

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
- Zu breite DbContext Lebenszeit (Singleton vermeiden)
- Falsche DeleteBehavior Einstellungen (Restrict vs. Cascade)
- Migrations nicht mit echten Daten testen 
- Entitäten als DTOs missbrauchen
- SaveChanges in Schleifen aufrufen (Bulk-Operations nutzen)
- Tracking bei Readonly-Abfragen aktiviert lassen
- Connection Strings in Code hardcoden statt Configuration nutzen
- Secrets in Source Control gespeichert
- Fehlende Nullable Reference Type Annotations
- Async/Await nicht konsequent verwenden
- Shadow Properties ohne Dokumentation verwenden
- Owned Entities vs. separate Tabellen falsch wählen
- Index-Strategien nicht an Abfrage Muster anpassen
- EF Core Version und Provider Version nicht synchron halten

## Testing
- Für schnelle Unit-Tests InMemory verwenden
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

