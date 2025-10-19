# Delete Behavior in EF Core – Optionen & Praxis (SQL Server)

> Konfiguration  
> ```csharp
> modelBuilder.Entity<Child>()
>   .HasOne(c => c.Parent)
>   .WithMany(p => p.Children)
>   .OnDelete(DeleteBehavior.ClientCascade); // Beispiel
> ```

## Übersicht

| Option (`DeleteBehavior`) | DB-FK (Constraint) | Was EF Core tut | Vorteile | Nachteile | Typische Verwendung |
|---|---|---|---|---|---|
| **Cascade** | `ON DELETE CASCADE` | Löscht abhängige Entities automatisch; DB cascaded auch für **nicht geladene** Rows. **Default** bei *required* Beziehungen. | Konsistent & simpel; funktioniert auch ohne getrackte Kinder. | **SQL Server**: „Multiple cascade paths"-Fehler häufig; große Deletes => viele Locks. | Komposition (z. B. Order => OrderLines), einfache Parent-Child-Beziehungen |
| **ClientCascade** | **kein** DB-Cascade (`NO ACTION`) | EF löscht **getrackte** Kinder automatisch; DB cascaded **nicht**. | **Ideal für SQL Server** bei komplexen Beziehungen; umgeht „multiple cascade paths"-Fehler; präzise Kontrolle im Code. | Greift nur für geladene/getrackte Kinder; ungeladene Rows blockieren via FK. | **Empfohlen** wenn DB-Cascade nicht möglich ist (z. B. komplexe Beziehungen, Vererbung) |
| **SetNull** | `ON DELETE SET NULL` | Setzt FK der Kinder auf `NULL` (FK muss nullable sein). | Parent löschen ohne Kind-Delete; Daten bleiben. | Verwaiste Datensätze möglich; Nachpflege nötig. Kann auch „multiple cascade paths"-Fehler auslösen. | Historien/Logs, optionale Beziehungen. |
| **ClientSetNull** | `NO ACTION` | EF setzt FK **nur bei getrackten** Kindern auf `null`; DB cascaded nicht. **Default** bei *optional* Beziehungen. | Keine DB-Seiteneffekte; gut für einfache Szenarien/Tests; **umgeht SQL Server-Einschränkungen**. | Ungeladene Rows bleiben => FK kann Delete verhindern. | Kleine Apps, prototypische Szenarien, optionale Beziehungen |
| **Restrict** | `NO ACTION` (SQL Server) | EF **verhindert Delete im ChangeTracker** bei geladenen Kindern; wirft `InvalidOperationException` **vor** `SaveChanges()`. DB verhindert zusätzlich. | **Doppelte Absicherung** (EF + DB); frühe, explizite Fehler; Delete mit Abhängigkeiten = Programmierfehler. | Funktioniert nur bei geladenen Entities; erfordert manuelle Löschreihenfolge. | Strenge Domänen (Stammdaten), wo Abhängigkeiten nie gelöscht werden dürfen. |
| **NoAction** | `NO ACTION` | EF **versucht**, FK auf `null` zu setzen **falls Änderungen im ChangeTracker** vorhanden sind; ansonsten macht EF nichts. DB verhindert Delete bei existierenden Kindern. | Klarer DB-Gatekeeper; **keine Cascade-Konflikte** auf SQL Server. | Verhalten schwer vorhersagbar; erfordert manuelle Löschreihenfolge. | Shared-DB/DB-First-Umgebungen, Legacy-Code. |
| **ClientNoAction** | `NO ACTION` | EF macht **absolut nichts** – weder löschen noch FK ändern. Du musst **alles manuell** im Code regeln. | Maximale Kontrolle im Code, keine stillen Seiteneffekte, **vorhersagbares Verhalten**. | Am leichtesten inkonsistent, wenn man nicht aufpasst. | Spezialfälle, Soft-Delete, eigene Geschäftslogik. |


## Wichtige Hinweise
- **Defaults:** Required => `Cascade`; Optional => `ClientSetNull`. Prüfe dein Modell/Provider, setze bei Bedarf explizit. 
- **Client\***-Varianten wirken **nur auf getrackte** Entities; ungeladene Kinder verhindert die **DB-Constraint** (kein Cascade).   
- Bei **SQL Server** mit komplexen Beziehungen bevorzuge **`ClientCascade`** oder **`NoAction`**.
- **Performance**: `Cascade` in DB ist meist schneller als `ClientCascade`, aber bei SQL Server oft nicht möglich

## Empfehlungen zur Auswahl:
- **Einfache Parent-Child-Beziehung** => `Cascade`.  
- **Komplexe Beziehungen / Multiple Paths** => `ClientCascade` (Entities laden, dann löschen).  
- **Parent löschen, Kinder behalten** => `SetNull` (FK nullable) 
- **Keine impliziten Deletes / Strenge Kontrolle** => `Restrict`/`NoAction`.  
- **Alles im Code regeln** (weder Delete noch nullen) => `ClientNoAction`.


###  Multiple Cascade Paths
SQL Server erlaubt **keine** Cascade-Deletes, wenn mehrere Kaskadenpfade zur gleichen Tabelle führen:

```
Parent1 ─(CASCADE)→ Child
Parent2 ─(CASCADE)→ Child  SQL Server Fehler!
```
**Lösungen:**
1. **`ClientCascade`** für einen/mehrere Pfade verwenden 
2. **`NoAction`** oder **`Restrict`** + manuelle Löschreihenfolge

