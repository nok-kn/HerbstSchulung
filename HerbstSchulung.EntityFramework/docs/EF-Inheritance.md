# EF Core: Inheritance

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

```
Start
│
└── Gibt es sehr hohe Anforderungen an Abfrage Performance?
      │
      ├── Ja
      │     └── Sind DB Constraints wichtig (NOT NULL) ?
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
```

## Andere Aspekte:
- Abfragemuster: Polymorphe Queries => TPH/TPT, Konkrete Queries => TPC
```
// Schlechte Performance bei TPC
var alleDokumente = await context.Set<Dokument>().ToListAsync();
//  Erfordert UNION ALL über mehrere Tabellen

// Beste Performance bei TPH
var allePersonen = await context.Set<Person>().ToListAsync();
//  Einfaches SELECT * FROM Persons

// Moderate Performance bei TPT
var alleFahrzeuge = await context.Set<Fahrzeug>().ToListAsync();
//  JOINs über Fahrzeuge + Autos + Lastkraftwagen
```

- Performance: Lesen: TPC > TPH > TPT,  Schreiben: TPH > TPC > TPT 
- Wartbarkeit: TPT = beste Erweiterbarkeit, TPH = einfachste Struktur
    
