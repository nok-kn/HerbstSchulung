# Vergleich: xUnit vs. MSTest v2

| **Kriterium**                   | **xUnit**                                             | **MSTest v2**                                          |
|--------------------------------|--------------------------------------------------------|--------------------------------------------------------|
| **Herkunft**                   | Community-getrieben, von NUnit-Mitentwicklern         | Microsoft, Teil des Visual Studio-Ökosystems           |
| **Test-Attribut**              | `[Fact]` für normale Tests<br>`[Theory]` für Datentests | `[TestMethod]`<br>`[DataTestMethod]` für Datentests    |
| **Setup / Teardown (Test-Ebene)** | Konstruktor = Setup<br>`IDisposable.Dispose()` = Teardown | `[TestInitialize]`<br>`[TestCleanup]`                 |
| **Setup / Teardown (Klassen-Ebene)** | `IClassFixture<T>` + Konstruktor                  | `[ClassInitialize]`<br>`[ClassCleanup]`               |
| **Datengestützte Tests**       | `[Theory]` + `[InlineData]`, `[MemberData]` usw.       | `[DataTestMethod]` + `[DataRow]`                      |
| **Assertions**                 | `Assert.Equal()`, `Assert.True()` etc. (nicht statisch) | `Assert.AreEqual()`, `Assert.IsTrue()` (statisch)     |
| **Parallelisierung**           | Integriert & konfigurierbar via `xunit.runner.json`   | Nur mit zusätzlicher Konfiguration möglich            |
| **Test-Erkennung (Discovery)** | Automatisch über `public` Klassen & `[Fact]`/`[Theory]` | Erfordert `[TestClass]` + `[TestMethod]`              |
| **Dependency Injection (DI)**  | Unterstützung via Konstruktor (z.B. `IClassFixture`)   | Keine native Unterstützung für Konstruktor-DI         |
| **Visual Studio Support**      | Ja (mit zusätzlichem Testadapter NuGet-Paket)         | Ja, nativ integriert                                   |
| **Cross-Plattform**            | Ja, vollständig .NET Core/.NET kompatibel             | Ja, seit MSTest v2 auch für .NET Core geeignet         |
| **Mocking-Unterstützung**      | keine eingbaut, kompatibel mit Moq, NSubstitute       | keine eingbaut, kompatibel mit Moq, NSubstitute       |
| **Typischer Einsatzbereich**   | Moderne, modulare, testgetriebene Projekte            | Klassische oder Microsoft-nahe Enterprise-Projekte     |

## Fazit

- **xUnit** eignet sich besonders gut für neue, moderne .NET-Projekte
- **MSTest v2** auch gut, vertraut für Teams mit Erfahrung mit klasischem .NET 

