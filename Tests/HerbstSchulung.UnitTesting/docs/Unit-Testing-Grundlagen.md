# Unit Testing – Grundlagen

## Ziele von Unit Tests
- Korrektheit einzelner Einheiten (Methoden/Klassen) sicherstellen
- Refactoring erleichtern (Sicherheitsnetz)
- Dokumentation des gewünschten Verhaltens (lebende Spezifikation)

## Testpyramide
- Unit Tests: automatisch, viele, schnell, isoliert, Infrastruktur gemockt
- Integrationstests: automatisch, weniger, echte Infrastruktur
- End-to-End, UI: wenige, manual oder automatisch, teuer

## Werkzeugkette
- Test-Framework: xUnit oder MS Test 2
- Assertions: FluentAssertions (ausdrucksstarke, lesbare Überprüfungen)
- Mocking: Moq oder NSubstitute 
- Runner: Microsoft.NET.Test.Sdk, xunit.runner.visualstudio

## Best Practices
- AAA-Muster (Arrange-Act-Assert) einhalten
- Ein erwartetes Verhalten pro Test prüfen (Given-When-Then im Namen ausdrücken)
- Testdaten klar und minimal halten
- Seiteneffekte mocken (Zeit, Datei, Netzwerk, Zufall)
- Flaky Tests vermeiden (keine Sleeps ohne Grund, feste Zeiten/Seeds)
- Schnelle Feedback-Loop (Tests lokal und in CI)
- Benenne Tests sprechend: Methode_Zustand_Erwartung
- Tests sollten unabhängig voneinander ausführbar sein
- Code testbar gestalten (Dependency Injection, keine statische Klassen)

## Was testen, wenn wenig Zeit ist (pragmatische Priorisierung)
von höchsten zu nidrigen Priorität:
- Happy Path: Wichtigster End-to-End Fluss der Kernfunktion(en)
- Kritische Geschäftsregeln: Geld/Abrechnung, Berechtigungen, Deadlines, Idempotenz (z.B. mehrfaches Ausführen von kritischen Funktion mit denselben Parametern und denselben Systemzustand)
- Eigene APIs für externe Systeme
- Eigene interne APIs 
- Fehlerwege: Verhalten bei ungültigen Eingaben oder fehlschlagenden Abhängigkeiten.
- Validierungen und Grenzen: null/leer, Min/Max-Längen, Enum-Bereiche, Zahlenbereiche, Zeiträume/Zeitzonen
- Regressionen: nach Refactoring
- Regressionen: Für jeden behobenen Bug einen Test hinzufügen
- Smoke pro externe Abhängigkeit: DB, Queue, HTTP – kann verbinden + einfachster Flow
- Edge Cases 


## Zusätzliche Ressourcen
- Microsoft Docs – Testen in .NET: https://learn.microsoft.com/dotnet/core/testing/
- xUnit.net Dokumentation: https://xunit.net/
- FluentAssertions – Docs: https://fluentassertions.com/
- Moq – GitHub README: https://github.com/moq/moq4
- Testpyramide (Martin Fowler): https://martinfowler.com/bliki/TestPyramid.html
- Unit Testing Best Practices (Microsoft): https://learn.microsoft.com/dotnet/core/testing/unit-testing-best-practices
