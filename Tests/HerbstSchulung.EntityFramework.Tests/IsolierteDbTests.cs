using FluentAssertions;
using HerbstSchulung.EntityFramework.DataModel;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace HerbstSchulung.EntityFramework.Tests
{
    // nach jedem Test wird die DB schnell zurückgesetzt (mit dem DbFixture)
    
    [Collection("db")]
    public class IsolierteDbTests :  IAsyncLifetime
    {
        private readonly DbFixture _dbFixture;
        
        public IsolierteDbTests(DbFixture dbFicture)
        {
            _dbFixture = dbFicture;
        }
        
        [Fact]
        public async Task AppDbContext_Can_Add_And_Retrieve_Student()
        {
            // Arrange
            using var sut = Arrange.CreateDbContext(false);
            var student = new Student
            {
                Id = "STU-001",
                Name = "Max Mustermann",
                Nationality = "DE",
                School = "Test Schule"
            };

            // Act
            await sut.MergeAsync(student);
            await sut.SaveChangesAsync();

            // Assert
            var actual = await sut.Students.FirstOrDefaultAsync(s => s.Id == student.Id);
            actual.Should().NotBeNull();
            actual!.Name.Should().Be(student.Name);
            actual.School.Should().Be(student.School);
            actual.Nationality.Should().Be(student.Nationality);
            actual.Art.Should().Be(PersonArt.Student);
        }

        [Fact]
        public async Task AppDbContext_Can_Add_And_Retrieve_Teacher()
        {
            // Arrange
            using var sut = Arrange.CreateDbContext(false);
            var teacher = new Teacher
            {
                Id = "TCH-001",
                Name = "Anna Schmidt",
                Nationality = "AT",
                Subject = "Mathematik"
            };

            // Act
            await sut.MergeAsync(teacher);
            await sut.SaveChangesAsync();

            // Assert
            var actual = await sut.Teachers.FirstOrDefaultAsync(t => t.Id == teacher.Id);
            actual.Should().NotBeNull();
            actual!.Name.Should().Be(teacher.Name);
            actual.Subject.Should().Be(teacher.Subject);
            actual.Nationality.Should().Be(teacher.Nationality);
            actual.Art.Should().Be(PersonArt.Teacher);
        }

        [Fact]
        public async Task AppDbContext_Can_Add_And_Retrieve_Rechnung_With_Geld()
        {
            // Arrange
            using var sut = Arrange.CreateDbContext(false);
            var rechnung = new Rechnung
            {
                Id = "RCH-001",
                Titel = "Testrechnung",
                Rechnungsnummer = "R-2024-001",
                ZahlungszielTage = 30,
                BetragNetto = 1000.00m,
                BetragBrutto = new Geld(1190.00m, "EUR")
            };

            // Act
            sut.Rechnungen.Add(rechnung);
            await sut.SaveChangesAsync();

            // Assert
            var actual = await sut.Set<Rechnung>().FirstOrDefaultAsync(r => r.Id == rechnung.Id);
            actual.Should().NotBeNull();
            actual!.Titel.Should().Be(rechnung.Titel);
            actual.Rechnungsnummer.Should().Be(rechnung.Rechnungsnummer);
            actual.ZahlungszielTage.Should().Be(30);
            actual.BetragNetto.Should().Be(1000.00m);
            actual.BetragBrutto.Should().NotBeNull();
            actual.BetragBrutto.Wert.Should().Be(1190.00m);
            actual.BetragBrutto.Waehrung.Should().Be("EUR");
        }


        [Fact]
        public async Task AppDbContext_Can_Add_And_Retrieve_Angebot_With_Geld()
        {
            // Arrange
            using var sut = Arrange.CreateDbContext(false);
            var angebot = new Angebot
            {
                Id = "ANG-001",
                Titel = "Angebot Software-Entwicklung",
                GueltigBisUtc = DateTime.UtcNow.AddDays(30),
                RabattProzent = 10.0,
                BetragNetto = 5000.00m,
                BetragBrutto = new Geld(5950.00m, "EUR")
            };

            // Act
            await sut.MergeAsync(angebot);
            await sut.SaveChangesAsync();

            // Assert
            var actual = await sut.Set<Angebot>().FirstOrDefaultAsync(a => a.Id == angebot.Id);
            actual.Should().NotBeNull();
            actual!.Titel.Should().Be(angebot.Titel);
            actual.RabattProzent.Should().Be(10.0);
            actual.BetragBrutto.Should().NotBeNull();
            actual.BetragBrutto.Wert.Should().Be(5950.00m);
            actual.BetragBrutto.Waehrung.Should().Be("EUR");
        }

        [Fact]
        public async Task AppDbContext_Can_Update_Geld_In_Rechnung()
        {
            // Arrange
            using var sut = Arrange.CreateDbContext(false);
            var rechnung = new Rechnung
            {
                Id = "RCH-003",
                Titel = "Rechnung zum Aktualisieren",
                Rechnungsnummer = "R-2024-003",
                ZahlungszielTage = 30,
                BetragNetto = 1000.00m,
                BetragBrutto = new Geld(1190.00m, "EUR")
            };

            await sut.MergeAsync(rechnung);
            await sut.SaveChangesAsync();

            // Act - Update BetragBrutto
            rechnung.BetragBrutto = new Geld(1500.00m, "USD");
            await sut.SaveChangesAsync();

            // Assert
            sut.ChangeTracker.Clear(); // Cache leeren, damit wir sicher die Daten aus der DB lesen
            var actual = await sut.Set<Rechnung>().FirstOrDefaultAsync(r => r.Id == rechnung.Id);
            actual.Should().NotBeNull();
            actual!.BetragBrutto.Wert.Should().Be(1500.00m);
            actual.BetragBrutto.Waehrung.Should().Be("USD");
        }

        // Wird vor jedem Test ausgeführt
        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }
        
        // Wird nach jedem Test ausgeführt
        public Task DisposeAsync()
        {
            return _dbFixture.ResetAsync();
        }
    }
}
