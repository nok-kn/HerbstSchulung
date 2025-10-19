# Delete Behavior in EF Core – Optionen & Praxis

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
| **Cascade** | `ON DELETE CASCADE` | Löscht abhängige Entities automatisch; DB cascaded auch für **nicht geladene** Rows. **Default** bei *required* Beziehungen. | Konsistent & simpel; funktioniert auch ohne getrackte Kinder. | „Multiple cascade paths“ auf SQL Server; große Deletes => viele Locks. | Komposition (z. B. Order => OrderLines)|
| **ClientCascade** | **kein** DB-Cascade (`NO ACTION`) | EF löscht **getrackte** Kinder automatisch; DB cascaded **nicht**. | Umgeht DB-Einschränkungen (z. B. multiple Pfade); präzise Kontrolle im Code. | Greift nur für geladene/getrackte Kinder; ungeladene Rows blockieren via FK. | Wenn DB-Cascade nicht möglich/gewünscht ist (z. B. komplexe Beziehungen) |
| **SetNull** | `ON DELETE SET NULL` | Setzt FK der Kinder auf `NULL` (FK muss nullable sein). | Parent löschen ohne Kind-Delete; Daten bleiben. | Verwaiste Datensätze möglich; Nachpflege nötig. | Historien/Logs, optionale Beziehungen. :contentReference[oaicite:3]{index=3} |
| **ClientSetNull** | `NO ACTION` | EF setzt FK **nur bei getrackten** Kindern auf `null`; DB cascaded nicht. **Default** bei *optional* Beziehungen. | Keine DB-Seiteneffekte; gut für einfache Szenarien/Tests. | Ungeladene Rows bleiben => FK kann Delete verhindern. | Kleine Apps, prototypische Szenarien |
| **Restrict** | `NO ACTION`/`RESTRICT` | Keine automatische Aktion; Delete des Parents scheitert, wenn Kinder existieren. | Harte Datenintegrität, erzwingt Löschreihenfolge. | Mehr Orchestrierung/Code nötig. | Strenge Domänen (Stammdaten). |
| **NoAction** | `NO ACTION` | Wie `Restrict` – EF setzt ggf. FKs auf `null` bei tracked Changes, DB verhindert sonst. (In vielen Providern äquivalent zu `Restrict`.) | Klarer DB-Gatekeeper. | Gleiche Trade-offs wie `Restrict`. | Shared-DB/DB-First-Umgebungen. |
| **ClientNoAction** | `NO ACTION` | EF löscht **nicht** und setzt FK **nicht** auf `null`; reine App-Verantwortung. | Maximale Kontrolle im Code, keine stillen Seiteneffekte. | Am leichtesten inkonsistent, wenn man nicht aufpasst. | Spezialfälle, Soft-Delete, eigene Geschäftslogik. |

## Wichtige Hinweise
- **Defaults:** Required => `Cascade`; Optional => `ClientSetNull`. Prüfe dein Modell/Provider, setze bei Bedarf explizit. 
- **Client\***-Varianten wirken **nur auf getrackte** Entities; ungeladene Kinder verhindert die **DB-Constraint** (kein Cascade).   
- Ausführliche Erläuterungen zu Kaskaden & „deleting orphans“: **Cascade Delete**-Artikel.

## Kurzrezepte
- DB-Cascade gewünscht => `Cascade`.  
- DB-Cascade **nicht** möglich (z. B. multiple paths) => `ClientCascade`.  
- Parent löschen, Kinder behalten => `SetNull` (FK nullable).  
- Keine impliziten Deletes => `Restrict`/`NoAction`.  
- Alles im Code regeln (weder Delete noch nullen) => `ClientNoAction`.

