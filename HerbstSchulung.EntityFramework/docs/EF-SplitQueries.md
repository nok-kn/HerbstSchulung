# EF Core: Split Queries

**Kurzbeschreibung:**  
Mit `AsSplitQuery()` (bzw. global über `UseQuerySplittingBehavior(SplitQuery)`) teilt EF Core eine komplexe `Include`-Abfrage in mehrere SQL-Statements auf.  
Das verhindert sogenannte *kartesische Explosionen* (viele redundante Zeilen bei `Include` auf Collections) und reduziert die Datenmenge, die von der Datenbank übertragen wird.  
Der Nachteil: Es entstehen mehrere Roundtrips zur Datenbank, und zwischen den Abfragen kann sich der Datenbestand ändern.

---

## Wann Split Queries verwenden / nicht verwenden

| Situation | Split Query verwenden | Grund | Nicht verwenden | Grund |
|------------|----------------------|--------|------------------|--------|
| `Include` auf großen Collections (z. B. `Orders.Include(o => o.Lines)`) | Ja | Vermeidet kartesische Explosion und reduziert Datenmenge |  |  |
| Mehrere `Include`-Pfade auf Collections | Ja | Verhindert exponentielles Wachstum durch mehrere Joins |  |  |
| Reine Leseabfragen, leichte Inkonsistenz tolerierbar | Ja | Konsistenz über mehrere Abfragen ist unkritisch |  |  |
| Hohe Netzwerklatenz (z. B. Cloud oder Remote-DB) |  | Mehrere Roundtrips sind teuer | Ja (SingleQuery) | Eine Abfrage ist effizienter |
| Strikte Momentaufnahme (Parent und Children müssen exakt übereinstimmen) |  | Zwischen den einzelnen Abfragen können sich Daten ändern | Ja (SingleQuery oder Transaktion) | Eine Abfrage garantiert konsistente Daten |
| Verwendung von `Skip`, `Take`, `Distinct`, `GroupBy`, `OrderBy` über Joins hinweg |  | Aufgeteilte Abfragen können semantische Unterschiede verursachen | Ja (SingleQuery) | Ein gemeinsamer Query-Plan liefert stabilere Ergebnisse |
| Kleine Datenmengen, kein Explosion-Risiko |  | Split verursacht unnötigen Overhead | Ja (SingleQuery) | Eine Abfrage ist einfacher und schneller |
| Komplexes Tracking über viele Entities |  | Mehrere Abfragen erhöhen die Nachbearbeitung im Change Tracker | Ja (SingleQuery) | Einheitliches Materialisieren in einem Durchlauf |

---

## Empffehlung

- pro Query steuern
```csharp
// Gegen Datenexplosion
ctx.Orders.Include(o => o.Lines).AsSplitQuery().ToListAsync();

// Eine Abfrage erzwingen
ctx.Orders.Include(o => o.Lines).ToListAsync();

- async bevorzugen
