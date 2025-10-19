# EF Core: Split Queries

**Kurzbeschreibung:**  
Mit `AsSplitQuery()` (bzw. global �ber `UseQuerySplittingBehavior(SplitQuery)`) teilt EF Core eine komplexe `Include`-Abfrage in mehrere SQL-Statements auf.  
Das verhindert sogenannte *kartesische Explosionen* (viele redundante Zeilen bei `Include` auf Collections) und reduziert die Datenmenge, die von der Datenbank �bertragen wird.  
Der Nachteil: Es entstehen mehrere Roundtrips zur Datenbank, und zwischen den Abfragen kann sich der Datenbestand �ndern.

---

## Wann Split Queries verwenden / nicht verwenden

| Situation | Split Query verwenden | Grund | Nicht verwenden | Grund |
|------------|----------------------|--------|------------------|--------|
| `Include` auf gro�en Collections (z. B. `Orders.Include(o => o.Lines)`) | Ja | Vermeidet kartesische Explosion und reduziert Datenmenge |  |  |
| Mehrere `Include`-Pfade auf Collections | Ja | Verhindert exponentielles Wachstum durch mehrere Joins |  |  |
| Reine Leseabfragen, leichte Inkonsistenz tolerierbar | Ja | Konsistenz �ber mehrere Abfragen ist unkritisch |  |  |
| Hohe Netzwerklatenz (z. B. Cloud oder Remote-DB) |  | Mehrere Roundtrips sind teuer | Ja (SingleQuery) | Eine Abfrage ist effizienter |
| Strikte Momentaufnahme (Parent und Children m�ssen exakt �bereinstimmen) |  | Zwischen den einzelnen Abfragen k�nnen sich Daten �ndern | Ja (SingleQuery oder Transaktion) | Eine Abfrage garantiert konsistente Daten |
| Verwendung von `Skip`, `Take`, `Distinct`, `GroupBy`, `OrderBy` �ber Joins hinweg |  | Aufgeteilte Abfragen k�nnen semantische Unterschiede verursachen | Ja (SingleQuery) | Ein gemeinsamer Query-Plan liefert stabilere Ergebnisse |
| Kleine Datenmengen, kein Explosion-Risiko |  | Split verursacht unn�tigen Overhead | Ja (SingleQuery) | Eine Abfrage ist einfacher und schneller |
| Komplexes Tracking �ber viele Entities |  | Mehrere Abfragen erh�hen die Nachbearbeitung im Change Tracker | Ja (SingleQuery) | Einheitliches Materialisieren in einem Durchlauf |

---

## Empffehlung

- pro Query steuern
```csharp
// Gegen Datenexplosion
ctx.Orders.Include(o => o.Lines).AsSplitQuery().ToListAsync();

// Eine Abfrage erzwingen
ctx.Orders.Include(o => o.Lines).ToListAsync();

- async bevorzugen
