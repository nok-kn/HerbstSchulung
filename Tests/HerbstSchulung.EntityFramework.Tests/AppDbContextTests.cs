using FluentAssertions;
using HerbstSchulung.EntityFramework.DataModel;
using HerbstSchulung.EntityFramework.Seeding;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HerbstSchulung.EntityFramework.Tests;

/// <summary>
/// Beispiel-Tests für AppDbContext.
/// </summary>
public class AppDbContextTests
{
    /// <summary>
    /// Hilfsmethode: Erstellt einen InMemory-DbContext pro Test (isoliert).
    /// </summary>
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }

    [Fact]
    public async Task AppDbContext_Can_Add_And_Retrieve_Student()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        var student = new Student
        {
            Id = "STU-001",
            Name = "Max Mustermann",
            Nationality = "DE",
            School = "Test Schule"
        };

        // Act
        db.Students.Add(student);
        await db.SaveChangesAsync();

        // Assert
        var retrievedStudent = await db.Students.FirstOrDefaultAsync(s => s.Id == "STU-001");
        retrievedStudent.Should().NotBeNull();
        retrievedStudent!.Name.Should().Be("Max Mustermann");
        retrievedStudent.School.Should().Be("Test Schule");
        retrievedStudent.Nationality.Should().Be("DE");
        retrievedStudent.Art.Should().Be(PersonArt.Student);
    }

    [Fact]
    public async Task AppDbContext_Can_Add_And_Retrieve_Teacher()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        var teacher = new Teacher
        {
            Id = "TCH-001",
            Name = "Anna Schmidt",
            Nationality = "AT",
            Subject = "Mathematik"
        };

        // Act
        db.Teachers.Add(teacher);
        await db.SaveChangesAsync();

        // Assert
        var retrievedTeacher = await db.Teachers.FirstOrDefaultAsync(t => t.Id == "TCH-001");
        retrievedTeacher.Should().NotBeNull();
        retrievedTeacher!.Name.Should().Be("Anna Schmidt");
        retrievedTeacher.Subject.Should().Be("Mathematik");
        retrievedTeacher.Nationality.Should().Be("AT");
        retrievedTeacher.Art.Should().Be(PersonArt.Teacher);
    }

    [Fact]
    public async Task AppDbContext_Can_Add_And_Retrieve_Auto()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        var auto = new Auto
        {
            Id = "AUT-001",
            Hersteller = "VW",
            Modell = "Golf",
            Baujahr = 2020,
            AnzahlTueren = 5,
            HatHybridantrieb = false
        };

        // Act
        db.Autos.Add(auto);
        await db.SaveChangesAsync();

        // Assert
        var retrievedAuto = await db.Autos.FirstOrDefaultAsync(a => a.Id == "AUT-001");
        retrievedAuto.Should().NotBeNull();
        retrievedAuto!.Hersteller.Should().Be("VW");
        retrievedAuto.Modell.Should().Be("Golf");
        retrievedAuto.Baujahr.Should().Be(2020);
        retrievedAuto.AnzahlTueren.Should().Be(5);
        retrievedAuto.HatHybridantrieb.Should().BeFalse();
    }

    [Fact]
    public async Task AppDbContext_Can_Add_And_Retrieve_Lastkraftwagen()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        var lkw = new Lastkraftwagen
        {
            Id = "LKW-001",
            Hersteller = "MAN",
            Modell = "TGX",
            Baujahr = 2019,
            ZuladungInTonnen = 25.5,
            Achsen = 3
        };

        // Act
        db.Lastkraftwagen.Add(lkw);
        await db.SaveChangesAsync();

        // Assert
        var retrievedLkw = await db.Lastkraftwagen.FirstOrDefaultAsync(l => l.Id == "LKW-001");
        retrievedLkw.Should().NotBeNull();
        retrievedLkw!.Hersteller.Should().Be("MAN");
        retrievedLkw.Modell.Should().Be("TGX");
        retrievedLkw.Baujahr.Should().Be(2019);
        retrievedLkw.ZuladungInTonnen.Should().Be(25.5);
        retrievedLkw.Achsen.Should().Be(3);
    }

    [Fact]
    public async Task AppDbContext_HasData_Seeds_Laender()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        
        // In-Memory DB muss EnsureCreated aufrufen, damit HasData funktioniert
        await db.Database.EnsureCreatedAsync();

        // Act
        var laender = await db.Laender.ToListAsync();

        // Assert
        laender.Should().HaveCount(3);
        laender.Should().Contain(l => l.Name == "Deutschland" && l.IsoCode == "DE");
        laender.Should().Contain(l => l.Name == "Österreich" && l.IsoCode == "AT");
        laender.Should().Contain(l => l.Name == "Schweiz" && l.IsoCode == "CH");
    }

    [Fact]
    public void TestData_EnsureSeeded_Creates_Persons_And_Vehicles()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        db.Database.EnsureCreated();

        // Act
        TestData.EnsureSeeded(db);

        // Assert
        var students = db.Students.ToList();
        var teachers = db.Teachers.ToList();
        var autos = db.Autos.ToList();
        var lkws = db.Lastkraftwagen.ToList();

        students.Should().HaveCountGreaterOrEqualTo(1);
        teachers.Should().HaveCountGreaterOrEqualTo(1);
        autos.Should().HaveCountGreaterOrEqualTo(1);
        lkws.Should().HaveCountGreaterOrEqualTo(1);
    }

    [Fact]
    public void TestData_EnsureSeeded_Does_Not_Duplicate_Data()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        db.Database.EnsureCreated();

        // Act
        TestData.EnsureSeeded(db);
        var initialPersonCount = db.Students.Count() + db.Teachers.Count();
        var initialVehicleCount = db.Set<Fahrzeug>().Count();

        TestData.EnsureSeeded(db); // zweiter Aufruf

        // Assert
        var finalPersonCount = db.Students.Count() + db.Teachers.Count();
        var finalVehicleCount = db.Set<Fahrzeug>().Count();

        finalPersonCount.Should().Be(initialPersonCount, "EnsureSeeded should not duplicate data");
        finalVehicleCount.Should().Be(initialVehicleCount, "EnsureSeeded should not duplicate data");
    }

    [Fact]
    public async Task EntityBase_Sets_CreatedUtc_Automatically()
    {
        // Arrange
        using var db = CreateInMemoryDbContext();
        var beforeCreate = DateTime.UtcNow.AddSeconds(-1);
        
        var student = new Student
        {
            Id = "STU-TIME-001",
            Name = "Zeit Test",
            School = "Test",
            Nationality = "DE"
        };

        // Act
        db.Students.Add(student);
        await db.SaveChangesAsync();
        var afterCreate = DateTime.UtcNow.AddSeconds(1);

        // Assert
        student.CreatedUtc.Should().BeAfter(beforeCreate);
        student.CreatedUtc.Should().BeBefore(afterCreate);
    }
}
