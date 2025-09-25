| **Prefix**   | **Bedeutung / Zweck**                                                  | **Typischer Rückgabewert**     | **Beispiel**                             |
| ------------ | ---------------------------------------------------------------------- | ------------------------------ | ---------------------------------------- |
| `Try`        | Versucht eine Operation (kann fehlschlagen), gibt Erfolgsstatus zurück | `bool` (mit `out`-Parameter)   | `TryParse(string eingabe, out int zahl)` |
| `Is`         | Prüft eine Bedingung (ist...)                                          | `bool`                         | `IsAngemeldet()`                         |
| `Has`        | Prüft auf Vorhandensein oder Besitz                                    | `bool`                         | `HasRechte(User benutzer)`               |
| `Can`        | Prüft, ob etwas möglich ist                                            | `bool`                         | `CanAusgeführtWerden(Befehl befehl)`     |
| `Get`        | Holt einen Wert oder ein Objekt (lesen)                                | Beliebiger Typ                 | `GetBenutzerNachId(int id)`              |
| `Set`        | Setzt oder verändert einen Wert                                        | `void`                         | `SetLogLevel(LogLevel level)`            |
| `Update`     | Aktualisiert einen bestehenden Zustand oder Daten                      | `void` oder `bool`             | `UpdateProfil(User user)`                |
| `Refresh`    | Lädt Daten neu oder synchronisiert                                     | `void`                         | `RefreshToken()`                         |
| `Validate`   | Prüft die Gültigkeit von Daten oder Regeln                             | `bool` oder `ValidationResult` | `ValidateEmail(string email)`            |
| `Execute`    | Führt eine Aktion aus (oft ein Befehl)                                 | `void`, `Task` oder Ergebnis   | `ExecuteCommand(Command cmd)`            |
| `Run`        | Startet einen Prozess oder Job                                         | `void` oder `Task`             | `RunBackupJob()`                         |
| `Create`     | Erstellt ein neues Objekt oder eine Instanz                            | Neues Objekt                   | `CreateBenutzer(string name)`            |
| `Build`      | Baut ein komplexes Objekt zusammen                                     | Komplexes Objekt               | `BuildReport(DateTime datum)`            |
| `Load`       | Lädt Daten aus Speicher/Datei/Datenbank                                | Beliebiger Typ                 | `LoadEinstellungen()`                    |
| `Save`       | Speichert Daten in Speicher/Datei/Datenbank                            | `void` oder `bool`             | `SaveDokument(Dokument dok)`             |
| `Calculate`  | Führt eine Berechnung durch                                            | Zahl oder Ergebnis             | `CalculateSteuer(Bestellung bestellung)` |
| `Clear`      | Leert einen Zustand oder eine Sammlung                                 | `void`                         | `ClearCache()`                           |
| `Reset`      | Setzt etwas auf den Ursprungszustand zurück                            | `void`                         | `ResetKonfiguration()`                   |
| `Initialize` | Initialisiert etwas vor der Benutzung                                  | `void`                         | `InitializeDatenbank()`                  |
| `Handle`     | Behandelt ein Ereignis oder eine Situation                             | `void` oder `Task`             | `HandleLoginFehler()`                    |
| `Process`    | Verarbeitet Daten oder eine Aktion                                     | `void` oder Ergebnis           | `ProcessZahlung(ZahlungInfo info)`       |
