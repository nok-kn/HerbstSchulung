# Interaktives Quiz zu .NET-Konzepten (leicht & gelegentlich witzig)

Anleitung
- SC = Single Choice (genau eine Antwort richtig)
- MC = Multiple Choice (eine oder mehrere Antworten richtig)
- Kreuze an, dann Lösung aufklappen

---

1) (SC) Wozu dient Dependency Injection (DI) primär?
- [ ] A. Modularität und Testbarkeit erhöhen
- [ ] B. Macht den Code automatisch schneller
- [ ] C. Verhindert alle Exceptions
- [ ] D. Ersetzt alle Design Patterns
<details><summary>Lösung</summary>
A — Bessere Entkopplung, Austauschbarkeit, Testbarkeit.
</details>

2) (MC) DI-Lifetimes – welche Zuordnung ist korrekt?
- [ ] A. Singleton: einmal pro Container
- [ ] B. Scoped: einmal pro Thread
- [ ] C. Scoped: einmal pro Anfrage/Scope
- [ ] D. Transient: bei jeder Auflösung neu

<details><summary>Lösung</summary>
A, C, D
</details>

3) (SC) Welche Options-Variante ist für Laufzeitänderungen gedacht?
- [ ] A. IOptions<T>
- [ ] B. IOptionsSnapshot<T>
- [ ] C. IOptionsMonitor<T>
- [ ] D. IOptionizer<T>
<details><summary>Lösung</summary>
C — Monitor beobachtet Änderungen und kann OnChange auslösen.
</details>

4) (MC) Mögliche Konfigurationsquellen im .NET-Host:
- [ ] A. JSON-Dateien
- [ ] B. Umgebungsvariablen
- [ ] C. Kommandozeilenargumente
- [ ] D. In-Memory-Dictionaries
<details><summary>Lösung</summary>
A, B, C, D — alles valide.
</details>

5) (SC) Strukturiertes Logging erkennt Parameter am besten durch…
- [ ] A. $"User {userId} startete {feature}"
- [ ] B. "User {UserId} startete {Feature}", userId, feature
- [ ] C. "User " + userId + " startete " + feature
- [ ] D. Console.WriteLine
<details><summary>Lösung</summary>
B — Platzhalter behalten Struktur in Sinks (Serilog/Seq etc.).
</details>

6) (SC) Task.Run sollte hauptsächlich verwendet werden, um…
- [ ] A. I/O‑gebundene async-Operationen schneller zu machen
- [ ] B. CPU‑gebundene Arbeit vom UI-/Request-Thread in den ThreadPool auszulagern
- [ ] C. Deadlocks grundsätzlich zu verhindern
- [ ] D. jede async-Methode „wirklich“ parallel zu machen
<details><summary>Lösung</summary>
B — Task.Run ist für CPU-bound Work sinnvoll (z. B. UI entlasten). Für echte I/O-Async nicht nötig und in Server-Code selten sinnvoll.
</details>

7) (SC) Was bewirkt ConfigureAwait(false) typischerweise?
- [ ] A. Beschleunigt alle Tasks um 200%
- [ ] B. Verhindert Deadlocks magisch
- [ ] C. Fortsetzung ohne Synchronisationskontext (gut für Bibliotheken)
- [ ] D. Schaltet Logging aus
<details><summary>Lösung</summary>
C — Kein Capture des Synchronisationskontexts.
</details>

8) (SC) async void ist geeignet für…
- [ ] A. API-Methoden
- [ ] B. Event-Handler
- [ ] C. Alles, weil’s kürzer ist
- [ ] D. Konstruktoren
<details><summary>Lösung</summary>
B — Sonst lieber async Task/Task<T>.
</details>

9) (MC) Gute Unit-Tests sind…
- [ ] A. Deterministisch
- [ ] B. Abhängig von Netzwerk/Dateisystem
- [ ] C. Schnell und isoliert
- [ ] D. Von der Uhrzeit abhängg  (Sonne raus, Tests grün)
<details><summary>Lösung</summary>
A, C — B eher für Integration. D vermeiden :)
</details>

10) (MC) NuGet-Verwaltung in Teams: Was sind empfohlene Praktiken?
- [ ] A. Zentrales Paketmanagement via Directory.Packages.props aktivieren
- [ ] B. Paketversionen in jeder csproj individuell pflegen
- [ ] C. Lockfile (packages.lock.json) nutzen und im CI mit --locked-mode restoren
- [ ] D. Entwicklungs-/Testpakete mit PrivateAssets="all" referenzieren
- [ ] E. Pakete aus dem Shared Framework zusätzlich als NuGet referenzieren
- [ ] F. Floating-Versionen (z. B. 1.2.*) in Produktiv-Repos bevorzugen
<details><summary>Lösung</summary>
A, C, D — B/E/F sind Anti-Patterns. Für ASP.NET-Core-Apps Shared Framework via FrameworkReference nutzen; Versionen zentral pinnen.
</details>

11) (SC) EF Core InMemory im Test…
- [ ] A. Erzwingt relationale Constraints wie SQL
- [ ] B. Ist schnell, nutzt aber LINQ-to-Objects-Semantik
- [ ] C. Macht keinen Sinn für Hardcore-Entwickler
- [ ] D. Führt automatisch Migrationen aus
<details><summary>Lösung</summary>
B — Gut für Logiktests; Relationales lieber gegen echte DB/Testcontainer.
</details>

12) (SC) Nullable Reference Types: Was bedeutet `string?` 
- [ ] A. „Kann nie null sein“
- [ ] B. „Kann null sein“
- [ ] C. „Compiler ignoriert alles“
- [ ] D. „Wird zum Value Type“
<details><summary>Lösung</summary>
B — Als Referenztyp nullbar annotiert.
</details>

13) (SC) Default-Implementierungen in Interfaces erlauben…
- [ ] A. Klassenvererbung ohne Klassen
- [ ] B. Evolution von Interfaces ohne alle Implementierungen sofort zu brechen
- [ ] C. Mehrfachvererbung von Klassen
- [ ] D. Exceptions zu verbergen
<details><summary>Lösung</summary>
B — Bin-kompatible Erweiterungen erleichtern.
</details>

14) (SC) Der .NET Generic Host bringt vor allem…
- [ ] A. Nur Logging
- [ ] B. Nur DI
- [ ] C. Lebenszyklus, DI, Konfiguration, Logging – alles aus einer Hand
- [ ] D. Kopfschmerzen
<details><summary>Lösung</summary>
C — Das Host-Grundgerüst bündelt zentrale Infrastruktur.
</details>

15) (MC) Beste Praxis für Cancellation in async-APIs:
- [ ] A. CancellationToken als Parameter akzeptieren
- [ ] B. Token an I/O-Aufrufe weiterreichen
- [ ] C. Token ignorieren (weniger Code!)
- [ ] D. Eigene Tokens in jeder Methode neu erzeugen
<details><summary>Lösung</summary>
A, B — C/D vermeiden.
</details>

16) (SC) Exceptions korrekt loggen bedeutet…
- [ ] A. _logger.LogError("Fehler: " + ex)
- [ ] B. _logger.LogError(ex, "Fehler bei \{Aktion\}", aktion)
- [ ] C. Console.WriteLine(ex.ToString())
- [ ] D. Gar nicht loggen, macht nur Sorgen
<details><summary>Lösung</summary>
B — Exception als erstes Argument, strukturierte Daten nutzen.
</details>

17) (MC) DI-Falle „Captive Dependency“ vermeiden:
- [ ] A. Singleton darf Scoped-Service injizieren
- [ ] B. Singleton sollte nur Singleton/konstante Abhängigkeiten halten
- [ ] C. Transient in Scoped ist ok
- [ ] D. Scoped in Transient ist immer verboten
<details><summary>Lösung</summary>
B, C — A führt zu Lifetimemismatch; D ist nicht generell verboten.
</details>

18) (SC) Validierung von Options über DataAnnotations…
- [ ] A. Geht nicht
- [ ] B. Über .ValidateDataAnnotations() bei der Registrierung
- [ ] C. Nur im Debug-Modus
- [ ] D. Führt nie zu Exceptions
<details><summary>Lösung</summary>
B — Bei Zugriff auf Value können OptionsValidationException auftreten.
</details>

---


