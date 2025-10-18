using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace HerbstSchulung.UnitTesting
{
    // Testklasse
    public class TestingExamples
    {
        // MemberData: Datengetriebener Test mit verschiedenen Uhrzeiten und erwarteten Grüßen
        public static IEnumerable<object[]> GrussCases => new List<object[]>
        {
            new object[] { 6, "Guten Morgen" },
            new object[] { 13, "Guten Tag" },
            new object[] { 20, "Guten Abend" },
        };

        [Theory]
        [MemberData(nameof(GrussCases))]
        // Klare Testnamen verwenden - nicht so:
        // public async Task TestCreateAuftrag(int stunde, string erwarteterGruss)
        public async Task CreateOrderAsync_Benachrichtigt_Mit_Korrektem_Gruss(int stunde, string erwarteterGruss)
        {
            // Arrange: Moq für Uhr und Notifier, EF InMemory für Repository, T
            var expectedDate = new DateTime(2024, 10, 1, stunde, 0, 0);
            var uhr = new Mock<IUhr>();
            uhr.SetupGet(u => u.Jetzt).Returns(expectedDate);
            var notifier = new Mock<INotifier>();
            using var repo = ArrangeRepository();

            var sut = new OrderService(uhr.Object, notifier.Object, repo);

            // Act
            var actual = await sut.CreateOrderAsync("kunde@example.org", "Laptop bestellen");

            // Assert: gespeichert + korrekter Benachrichtigungstext
            actual.Should().NotBeNull();
            actual.AngelegtAm.Should().Be(expectedDate);

            var alle = await repo.GetAllAsync();
            alle.Should().HaveCount(1);
            alle.Single().Beschreibung.Should().Be("Laptop bestellen");

            // white box testing: Prüfe, dass der Notifier mit dem erwarteten Gruß aufgerufen wurde
            notifier.Verify(n => n.SendeAsync(
                "kunde@example.org",
                It.Is<string>(t => t.StartsWith(erwarteterGruss)),
                It.IsAny<CancellationToken>()), Times.Once);
        }

        // negativer Test: Leerer Kunde führt zu Exception, keine Benachrichtigung
        [Fact]
        public async Task CreateOrderAsync_WirftException_Bei_Leerem_Kunden()
        {
            // Arrange
            var uhr = new Mock<IUhr>();
            uhr.SetupGet(u => u.Jetzt).Returns(new DateTime(2024, 10, 1, 9, 0, 0));
            var notifier = new Mock<INotifier>();
            using var repo = ArrangeRepository();
            var sut = new OrderService(uhr.Object, notifier.Object, repo);

            // Act
            var act = async () => await sut.CreateOrderAsync("", "Test");

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithParameterName("kunde")
                .WithMessage("*nicht leer*");

            // Keine Benachrichtigung gesendet
            notifier.VerifyNoOtherCalls();
        }

        

        // Beispiel: Test absichtlich überspringen (z. B. wegen externem System, noch nicht verfügbar)
        [Fact(Skip = "Beispiel: Dieser Test wird aktuell übersprungen, z. B. wegen fehlender Infrastruktur.")]
        public void Uebersprungener_Test_Beispiel()
        {
            // Hier würde ein Test stehen
            // Durch das Skip-Attribut wird er markiert, aber nicht ausgeführt.
        }

        // TDD-Beispiel: Zuerst den Test für CancelOrderAsync schreiben (Rot), dann Implementierung (Grün)
        [Fact]
        public async Task CancelOrderAsync_Existierender_Auftrag_Wird_Geloescht_Und_Benachrichtigung_Gesendet()
        {
            // Arrange (Red-Phase): Uhr/Notifier mocken, Repo befüllen
            var zeitpunkt = new DateTime(2024, 10, 1, 10, 0, 0);
            var uhr = new Mock<IUhr>();
            uhr.SetupGet(u => u.Jetzt).Returns(zeitpunkt);
            var notifier = new Mock<INotifier>();
            using var repo = ArrangeRepository();
            var sut = new OrderService(uhr.Object, notifier.Object, repo);

            var angelegt = await sut.CreateOrderAsync("alice@example.org", "Headset");
            var alleVorher = await repo.GetAllAsync();
            alleVorher.Should().HaveCount(1);

            // Act: zu stornierender Auftrag nach Id
            var actual = await sut.CancelOrderAsync(angelegt.Id);

            // Assert: Erfolg, Auftrag entfernt, Benachrichtigung gesendet
            actual.Should().BeTrue();
            var alleNachher = await repo.GetAllAsync();
            alleNachher.Should().BeEmpty();

            notifier.Verify(n => n.SendeAsync(
                "alice@example.org",
                It.Is<string>(t => t.Contains("storniert", StringComparison.OrdinalIgnoreCase)),
                It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        // Hilfsmethode: Erstellt einen InMemory-DbContext pro Test (isoliert)
        private static OrdersDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<OrdersDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new OrdersDbContext(options);
        }

        // Hilfsmethode: Erstellt eine Repository-Instanz (InMemory oder echte DB)
        private EfRepository<Auftrag> ArrangeRepository(bool inMemory = true)
        {	
            if (inMemory)
            {
                // InMemory ist super für schnelle, logikfokussierte Unit-Tests ohne relationale Anforderungen
                // aber InMemory nutzt LINQ-to-Objects Semantik und lässt inkonsistente Zustände durch, die in SQL scheitern würden
                var db = CreateInMemoryDbContext();
                return new EfRepository<Auftrag>(db);
            }
            else
            {
                // sobald Relationalität, SQL, Constraints oder komplexe Abfragen eine Rolle spielen (also fast immer :) ) , nimm eine echte DB oder Testcontainer
                // benutze [Trait("Category", "Integration")] um Integrationstests zu markieren
                throw new NotImplementedException("Hier kann man eine DB Repository für Integration test einbauen");
            }
        }
    }

    #region Abstraktionen / Modelle / Implementierungen

    public interface IUhr
    {
        DateTime Jetzt { get; }
    }

    public interface INotifier
    {
        Task SendeAsync(string empfaenger, string text, CancellationToken cancellationToken = default);
    }

    public interface IRepository<T> : IDisposable where T : class
    {
        Task AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }

    public class Auftrag
    {
        public int Id { get; set; }
        public string Kunde { get; set; } = string.Empty;
        public DateTime AngelegtAm { get; set; }
        public string Beschreibung { get; set; } = string.Empty;
    }

    public class OrdersDbContext : DbContext
    {
        public OrdersDbContext(DbContextOptions<OrdersDbContext> options) : base(options) { }
        public DbSet<Auftrag> Auftraege => Set<Auftrag>();
    }

    public sealed class EfRepository<T>(DbContext _db) : IRepository<T> where T : class
    {
        public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            await _db.Set<T>().AddAsync(entity, cancellationToken);
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _db.Set<T>().AsNoTracking().ToListAsync(cancellationToken);
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) => _db.SaveChangesAsync(cancellationToken);

        public void Dispose()
        {
            _db.Dispose();
        }
    }

    // Service mit 3 Abhängigkeiten
    public class OrderService(IUhr uhr, INotifier notifier, IRepository<Auftrag> repo)
    {

        // Legt einen Auftrag an, speichert ihn und benachrichtigt den Kunden mit zeitabhängiger Grußformel
        public async Task<Auftrag> CreateOrderAsync(string kunde, string beschreibung, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(kunde))
                throw new ArgumentException("Kunde darf nicht leer sein.", nameof(kunde));
            if (string.IsNullOrWhiteSpace(beschreibung))
                throw new ArgumentException("Beschreibung darf nicht leer sein.", nameof(beschreibung));

            var auftrag = new Auftrag
            {
                Kunde = kunde,
                Beschreibung = beschreibung,
                AngelegtAm = uhr.Jetzt
            };

            await repo.AddAsync(auftrag, cancellationToken);
            var changed = await repo.SaveChangesAsync(cancellationToken);

            // Nur benachrichtigen, wenn gespeichert wurde
            if (changed > 0)
            {
                var gruss = CreateGruss();
                var text = $"{gruss} {kunde}, Ihr Auftrag wurde erfasst.";
                await notifier.SendeAsync(kunde, text, cancellationToken);
            }

            return auftrag;
        }

        private string CreateGruss()
        {
            var stunde = uhr.Jetzt.Hour;
            var gruss = stunde switch
            {
                >= 5 and < 12 => "Guten Morgen",
                >= 12 and < 18 => "Guten Tag",
                _ => "Guten Abend"
            };
            return gruss;
        }

        public Task<bool> CancelOrderAsync(int id, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException($"Implementierung von {nameof(CancelOrderAsync)} noch nicht fertig");
        }
    }

    #endregion

}
