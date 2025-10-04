# HerbstSchulung.Async

Workshop-Modul: Asynchrone Programmierung in .NET (C# 12/13, .NET 8)

Inhalt:
- Anti-Beispiele mit typischen Fehlern (async void, fehlendes await, .Result/Wait-Blockierung, falsche Fehlerbehandlung)
- Herausforderungen

Siehe Dateien:
- AsyncExamples.cs – Anti-Beispiele mit Kommentaren
- AsyncChallenges.cs – Aufgaben
- Tests – xUnit-Tests zur Überprüfung

Weiterführende Ressourcen:
- Microsoft Docs: Asynchronous programming with async and await – https://learn.microsoft.com/dotnet/csharp/asynchronous-programming/
- Best Practices: Async/Await – https://learn.microsoft.com/dotnet/standard/async-in-depth
- ValueTask – https://learn.microsoft.com/dotnet/api/system.threading.tasks.valuetask
- IAsyncDisposable – https://learn.microsoft.com/dotnet/standard/garbage-collection/implementing-disposeasync
- ConfigureAwait – https://devblogs.microsoft.com/dotnet/configureawait-faq/
