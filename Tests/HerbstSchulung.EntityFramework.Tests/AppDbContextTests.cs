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
    /// Verwendet die neue IAppDbContextFactory-Implementierung für Tests.
    /// </summary>
    private static AppDbContext CreateInMemoryDbContext()
    {
        var factory = new InMemoryAppDbContextFactory();
        return factory.CreateContext();
    }

    [Fact]
    public async Task AppDbContext_Can_Add_And_Retrieve_Student()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        var student = new Student
        {
            Id = "STU-001",
            Name = "Max Mustermann",
            Nationality = "DE",
            School = "Test Schule"
        };

        // Act
        sut.Students.Add(student);
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
        using var sut = CreateInMemoryDbContext();
        var teacher = new Teacher
        {
            Id = "TCH-001",
            Name = "Anna Schmidt",
            Nationality = "AT",
            Subject = "Mathematik"
        };

        // Act
        sut.Teachers.Add(teacher);
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
    public async Task AppDbContext_Can_Add_And_Retrieve_Auto()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
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
        sut.Autos.Add(auto);
        await sut.SaveChangesAsync();

        // Assert
        var actual = await sut.Autos.FirstOrDefaultAsync(a => a.Id == auto.Id);
        actual.Should().NotBeNull();
        actual!.Hersteller.Should().Be(auto.Hersteller);
        actual.Modell.Should().Be(auto.Modell);
        actual.Baujahr.Should().Be(auto.Baujahr);
        actual.AnzahlTueren.Should().Be(auto.AnzahlTueren);
        actual.HatHybridantrieb.Should().Be(auto.HatHybridantrieb);
    }

    [Fact]
    public async Task AppDbContext_Can_Add_And_Retrieve_Lastkraftwagen()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
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
        sut.Lastkraftwagen.Add(lkw);
        await sut.SaveChangesAsync();

        // Assert
        var actual = await sut.Lastkraftwagen.FirstOrDefaultAsync(l => l.Id == lkw.Id);
        actual.Should().NotBeNull();
        actual!.Hersteller.Should().Be(lkw.Hersteller);
        actual.Modell.Should().Be(lkw.Modell);
        actual.Baujahr.Should().Be(lkw.Baujahr);
        actual.ZuladungInTonnen.Should().Be(lkw.ZuladungInTonnen);
        actual.Achsen.Should().Be(lkw.Achsen);
    }

    [Fact]
    public async Task AppDbContext_HasData_Seeds_Laender()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        
        // In-Memory DB muss EnsureCreated aufrufen, damit HasData funktioniert
        await sut.Database.EnsureCreatedAsync();

        // Act
        var actual = await sut.Laender.ToListAsync();

        // Assert
        actual.Should().HaveCount(3);
        actual.Should().Contain(l => l.Name == "Deutschland" && l.IsoCode == "DE");
        actual.Should().Contain(l => l.Name == "Österreich" && l.IsoCode == "AT");
        actual.Should().Contain(l => l.Name == "Schweiz" && l.IsoCode == "CH");
    }

    [Fact]
    public void TestData_EnsureSeeded_Creates_Persons_And_Vehicles()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        sut.Database.EnsureCreated();

        // Act
        TestData.EnsureSeeded(sut);

        // Assert
        var actualStudents = sut.Students.ToList();
        var actualTeachers = sut.Teachers.ToList();
        var actualAutos = sut.Autos.ToList();
        var actualLkws = sut.Lastkraftwagen.ToList();

        actualStudents.Should().HaveCountGreaterOrEqualTo(1);
        actualTeachers.Should().HaveCountGreaterOrEqualTo(1);
        actualAutos.Should().HaveCountGreaterOrEqualTo(1);
        actualLkws.Should().HaveCountGreaterOrEqualTo(1);
    }

    [Fact]
    public void TestData_EnsureSeeded_Does_Not_Duplicate_Data()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        sut.Database.EnsureCreated();

        // Act
        TestData.EnsureSeeded(sut);
        var initialPersonCount = sut.Students.Count() + sut.Teachers.Count();
        var initialVehicleCount = sut.Set<Fahrzeug>().Count();

        TestData.EnsureSeeded(sut); // zweiter Aufruf

        // Assert
        var actualPersonCount = sut.Students.Count() + sut.Teachers.Count();
        var actualVehicleCount = sut.Set<Fahrzeug>().Count();

        actualPersonCount.Should().Be(initialPersonCount, "EnsureSeeded should not duplicate data");
        actualVehicleCount.Should().Be(initialVehicleCount, "EnsureSeeded should not duplicate data");
    }

    [Fact]
    public async Task EntityBase_Sets_CreatedUtc_Automatically()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        var beforeCreate = DateTime.UtcNow.AddSeconds(-1);
        
        var student = new Student
        {
            Id = "STU-TIME-001",
            Name = "Zeit Test",
            School = "Test",
            Nationality = "DE"
        };

        // Act
        sut.Students.Add(student);
        await sut.SaveChangesAsync();
        var afterCreate = DateTime.UtcNow.AddSeconds(1);

        // Assert
        student.CreatedUtc.Should().BeAfter(beforeCreate);
        student.CreatedUtc.Should().BeBefore(afterCreate);
    }

    [Fact]
    public void Factory_Creates_Isolated_Contexts_By_Default()
    {
        // Arrange
        var sut = new InMemoryAppDbContextFactory();
        var student = new Student 
        { 
            Id = "STU-ISOLATED-001", 
            Name = "Test", 
            School = "School", 
            Nationality = "DE" 
        };
        
        // Act
        using var context1 = sut.CreateContext();
        context1.Students.Add(student);
        context1.SaveChanges();

        using var context2 = sut.CreateContext();
        var actual = context2.Students.FirstOrDefault(s => s.Id == student.Id);

        // Assert
        actual.Should().BeNull("Each context should have its own isolated database");
    }

    [Fact]
    public void Factory_CreateShared_Shares_Database_Between_Contexts()
    {
        // Arrange
        var sut = InMemoryAppDbContextFactory.CreateShared();
        var student = new Student 
        { 
            Id = "STU-SHARED-001", 
            Name = "Test", 
            School = "School", 
            Nationality = "DE" 
        };
        
        // Act
        using (var context1 = sut.CreateContext())
        {
            context1.Database.EnsureCreated();
            context1.Students.Add(student);
            context1.SaveChanges();
        }

        using (var context2 = sut.CreateContext())
        {
            var actual = context2.Students.FirstOrDefault(s => s.Id == student.Id);
            
            // Assert
            actual.Should().NotBeNull("Shared factory should allow data access across contexts");
            actual!.Name.Should().Be(student.Name);
        }
    }

    [Fact]
    public void Factory_CreateReadOnlyContext_Uses_NoTracking()
    {
        // Arrange
        var sut = InMemoryAppDbContextFactory.CreateShared();
        var student = new Student 
        { 
            Id = "STU-READONLY-001", 
            Name = "Original", 
            School = "School", 
            Nationality = "DE" 
        };
        
        using var writeContext = sut.CreateContext();
        writeContext.Database.EnsureCreated();
        writeContext.Students.Add(student);
        writeContext.SaveChanges();

        // Act
        using var readContext = sut.CreateReadOnlyContext();
        var actual = readContext.Students.First(s => s.Id == student.Id);
        actual.Name = "Modified";
        readContext.SaveChanges();

        // Assert - Verify NoTracking by checking if change was persisted
        using var verifyContext = sut.CreateContext();
        var verifiedStudent = verifyContext.Students.First(s => s.Id == student.Id);
        verifiedStudent.Name.Should().Be(student.Name, "ReadOnly context should use NoTracking and not persist changes");
    }
}
