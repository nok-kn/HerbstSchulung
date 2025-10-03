# Asynchron in .NET

## Kernidee von async/await
- async/await vereinfacht asynchrone, nicht-blockierende Abl�ufe.
- Wichtiger Punkt: async bedeutet nicht automatisch �neuer Thread�. Meist wird kein zus�tzlicher Thread gestartet.
- await pausiert die Methode, gibt die Kontrolle frei und setzt sp�ter fort, wenn das Ergebnis da ist.
- Besonders sinnvoll bei I/O-gebundenen Arbeiten (Netzwerk, Dateisystem, Datenbank), damit Threads nicht blockieren.

### Sequenz: I/O-gebundenes await (kein zus�tzlicher Thread notwendig)
```mermaid
sequenceDiagram
    participant Caller as Aufrufer (UI/Request)
    participant Methode as Async-Methode
    participant IO as I/O (Netzwerk/Datei)

    Caller->>Methode: Aufruf()
    activate Methode
    Methode->>IO: Lesen/Schreiben asynchron starten
    Note over Methode: await � Methode gibt Kontrolle zur�ck
    Methode-->>Caller: Task (noch nicht abgeschlossen)
    deactivate Methode

    IO-->>Methode: Ergebnis verf�gbar
    Note over Methode: Fortsetzung (Continuation)
    Methode-->>Caller: Task abgeschlossen (Resultat)
```

Hinweis: Es wird kein zus�tzlicher Thread f�r das Warten ben�tigt. Der Aufrufer-Thread bleibt frei f�r andere Arbeit (z. B. UI-Interaktion, weitere Requests).

## Async ist nicht gleich Multithreading
- Async/await modelliert �Warten ohne Blockieren�, nicht �Berechnung auf mehreren Threads�.
- I/O-Operationen brauchen prim�r Zeit, nicht CPU. W�hrend des Wartens sollte kein Thread blockieren.
- Multithreading verteilt CPU-Arbeit auf mehrere Threads/CPU-Kerne. Das erreichst du nicht automatisch mit await.
- Fortsetzungen (nach await) laufen standardm��ig im aktuellen Synchronisationskontext weiter (z. B. UI-Thread), au�er man nutzt ConfigureAwait(false) oder es gibt keinen Kontext (ASP.NET Core).

## Was macht Task.Run?
- Task.Run startet eine Arbeit im ThreadPool (neuer Hintergrund-Thread aus dem Pool).
- Geeignet f�r CPU-gebundene Arbeit, wenn der aktuelle Thread nicht blockiert werden darf (z. B. UI-Thread).
- Nicht geeignet als �Allzweck-Async-Schalter�. F�r I/O-gebundene Arbeit bringt Task.Run keinen Vorteil und erzeugt unn�tige Threads/Overhead.
- In ASP.NET Core ist Task.Run selten n�tig; der Request-Thread ist kein UI-Thread und sollte durch asynchrone I/O-Aufrufe frei bleiben.

### Kurzbeispiele
- I/O-bound (ohne Task.Run):
  - await httpClient.GetStringAsync(uri)
- CPU-bound (mit Task.Run, z. B. im UI-Kontext):
  - var result = await Task.Run(() => TeureBerechnung(eingabe))

## Best Practices
- Bei I/O: immer native Async-APIs nutzen und awaiten; keine .Result/.Wait().
- Bibliothekscode: ConfigureAwait(false) verwenden, um Synchronisationskontexte nicht einzufangen.
- Task.Run nur f�r CPU-arbeitende Abschnitte einsetzen, nicht f�r I/O.
- CancellationToken anbieten und respektieren.
- Exceptions nicht verschlucken; in async-Methoden Task statt async void verwenden (au�er Event-Handler).
- ValueTask nur gezielt f�r synchrone Schnellpfade/API-Hotpaths verwenden.

## Weiterf�hrende Ressourcen
- Asynchronous programming with async and await: https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/
- Async/Await best practices: https://learn.microsoft.com/dotnet/standard/async-in-depth
- ConfigureAwait FAQ: https://devblogs.microsoft.com/dotnet/configureawait-faq/
- ThreadPool & Task.Run: https://learn.microsoft.com/dotnet/standard/threading/the-managed-thread-pool
