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

        actualPersonCount.Should().Be(initialPersonCount, "EnsureSeeded sollte keine Duplikate erstellen");
        actualVehicleCount.Should().Be(initialVehicleCount, "EnsureSeeded sollte keine Duplikate erstellen");
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
        actual.Should().BeNull("Jeder Kontext sollte eine eigene isolierte Datenbank haben");
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
            actual.Should().NotBeNull("Geteilte Factory sollte den Datenzugriff zwischen Kontexten ermöglichen");
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
        var act = () => readContext.SaveChanges();

        // Assert - Überprüfen, dass SaveChanges eine Ausnahme auslöst
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*read-only*");
    }

    [Fact]
    public async Task Factory_CreateReadOnlyContext_SaveChangesAsync_Throws_Exception()
    {
        // Arrange
        var sut = InMemoryAppDbContextFactory.CreateShared();
        var student = new Student 
        { 
            Id = "STU-READONLY-002", 
            Name = "Original", 
            School = "School", 
            Nationality = "DE" 
        };
        
        using var writeContext = sut.CreateContext();
        await writeContext.Database.EnsureCreatedAsync();
        writeContext.Students.Add(student);
        await writeContext.SaveChangesAsync();

        // Act
        using var readContext = sut.CreateReadOnlyContext();
        var actual = await readContext.Students.FirstAsync(s => s.Id == student.Id);
        actual.Name = "Modified";
        var act = async () => await readContext.SaveChangesAsync();

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*read-only*");
    }

    [Fact]
    public async Task AddIfNotExistsAsync_Adds_New_Entity()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        var student = new Student
        {
            Id = "STU-NOTEXISTS-001",
            Name = "New Student",
            School = "Test School",
            Nationality = "DE"
        };

        // Act
        var actual = await sut.AddIfNotExistsAsync(student);
        await sut.SaveChangesAsync();

        // Assert
        actual.Should().BeTrue("Entity sollte hinzugefügt werden, wenn sie noch nicht existiert");
        var saved = await sut.Students.FirstOrDefaultAsync(s => s.Id == student.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be(student.Name);
    }

    [Fact]
    public async Task AddIfNotExistsAsync_Returns_False_For_Existing_Entity()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        var student = new Student
        {
            Id = "STU-EXISTS-001",
            Name = "Existing Student",
            School = "Test School",
            Nationality = "DE"
        };

        // Entity zuerst hinzufügen
        sut.Students.Add(student);
        await sut.SaveChangesAsync();

        // Act
        var newStudent = new Student
        {
            Id = student.Id,
            Name = "Different Name",
            School = "Different School",
            Nationality = "AT"
        };
        var actual = await sut.AddIfNotExistsAsync(newStudent);
        await sut.SaveChangesAsync();

        // Assert
        actual.Should().BeFalse("Entity sollte nicht hinzugefügt werden, wenn sie bereits existiert");
        var saved = await sut.Students.FirstOrDefaultAsync(s => s.Id == student.Id);
        saved!.Name.Should().Be("Existing Student", "Ursprüngliche Entity sollte unverändert bleiben");
    }

    [Fact]
    public async Task AddIfNotExistsAsync_Works_With_Different_Entity_Types()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        var auto = new Auto
        {
            Id = "AUT-NOTEXISTS-001",
            Hersteller = "Tesla",
            Modell = "Model 3",
            Baujahr = 2023,
            AnzahlTueren = 4,
            HatHybridantrieb = false
        };

        // Act
        var actual = await sut.AddIfNotExistsAsync(auto);
        await sut.SaveChangesAsync();

        // Assert
        actual.Should().BeTrue("Auto sollte hinzugefügt werden, wenn es noch nicht existiert");
        var saved = await sut.Autos.FirstOrDefaultAsync(a => a.Id == auto.Id);
        saved.Should().NotBeNull();
        saved!.Hersteller.Should().Be(auto.Hersteller);
    }

    [Fact]
    public async Task AddIfNotExistsAsync_Throws_ArgumentNullException_For_Null_Entity()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();

        // Act
        var act = async () => await sut.AddIfNotExistsAsync<Student>(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("entity");
    }

    [Fact]
    public async Task MergeAsync_Adds_New_Entity()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        var student = new Student
        {
            Id = "STU-MERGE-NEW-001",
            Name = "New Student",
            School = "Test School",
            Nationality = "DE"
        };

        // Act
        var actual = await sut.MergeAsync(student);
        await sut.SaveChangesAsync();

        // Assert
        actual.Should().BeTrue("Neue Entity sollte hinzugefügt werden");
        var saved = await sut.Students.FirstOrDefaultAsync(s => s.Id == student.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be(student.Name);
    }

    [Fact]
    public async Task MergeAsync_Updates_Tracked_Entity()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        var student = new Student
        {
            Id = "STU-TRACKED-001",
            Name = "Original Name",
            School = "Original School",
            Nationality = "DE"
        };

        // Entity hinzufügen und im Context tracken
        sut.Students.Add(student);
        await sut.SaveChangesAsync();

        // Act - Versuche dieselbe Entity mit geänderten Daten zu mergen
        var updatedStudent = new Student
        {
            Id = student.Id,
            Name = "Updated Name",
            School = "Updated School",
            Nationality = "AT"
        };
        var actual = await sut.MergeAsync(updatedStudent);

        // Assert
        actual.Should().BeFalse("Entity ist bereits getrackt und sollte aktualisiert werden");
        
        // Prüfe, dass die getrackten Werte aktualisiert wurden
        student.Name.Should().Be("Updated Name");
        student.School.Should().Be("Updated School");
        student.Nationality.Should().Be("AT");
    }

    [Fact]
    public async Task MergeAsync_Updates_Existing_Entity_In_Database()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        var student = new Student
        {
            Id = "STU-DBEXISTS-001",
            Name = "Original Name",
            School = "Original School",
            Nationality = "DE"
        };

        // Entity in DB speichern
        sut.Students.Add(student);
        await sut.SaveChangesAsync();
        
        // Context leeren, damit Entity nicht mehr getrackt wird
        sut.ChangeTracker.Clear();

        // Act - Versuche dieselbe Entity mit geänderten Daten zu mergen
        var updatedStudent = new Student
        {
            Id = student.Id,
            Name = "Updated Name",
            School = "Updated School",
            Nationality = "AT"
        };
        var actual = await sut.MergeAsync(updatedStudent);
        await sut.SaveChangesAsync();

        // Assert
        actual.Should().BeFalse("Entity existiert bereits in DB und sollte aktualisiert werden");
        
        // Prüfe, dass die Werte in der DB aktualisiert wurden
        var saved = await sut.Students.FirstOrDefaultAsync(s => s.Id == student.Id);
        saved.Should().NotBeNull();
        saved!.Name.Should().Be("Updated Name");
        saved.School.Should().Be("Updated School");
        saved.Nationality.Should().Be("AT");
    }

    [Fact]
    public async Task MergeAsync_Multiple_Entities()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        
        var student1 = new Student
        {
            Id = "STU-MERGE-001",
            Name = "Student 1",
            School = "School A",
            Nationality = "DE"
        };

        var student2 = new Student
        {
            Id = "STU-MERGE-002",
            Name = "Student 2",
            School = "School B",
            Nationality = "AT"
        };

        // Act - Erstes Hinzufügen
        var added1 = await sut.MergeAsync(student1);
        var added2 = await sut.MergeAsync(student2);
        await sut.SaveChangesAsync();

        // Aktualisierte Versionen
        var updated1 = new Student
        {
            Id = "STU-MERGE-001",
            Name = "Updated Student 1",
            School = "Updated School A",
            Nationality = "CH"
        };

        var updated2 = new Student
        {
            Id = "STU-MERGE-002",
            Name = "Updated Student 2",
            School = "Updated School B",
            Nationality = "DE"
        };

        var merged1 = await sut.MergeAsync(updated1);
        var merged2 = await sut.MergeAsync(updated2);
        await sut.SaveChangesAsync();

        // Assert
        added1.Should().BeTrue("Erste Entity sollte hinzugefügt werden");
        added2.Should().BeTrue("Zweite Entity sollte hinzugefügt werden");
        merged1.Should().BeFalse("Erste Entity sollte aktualisiert werden");
        merged2.Should().BeFalse("Zweite Entity sollte aktualisiert werden");

        var allStudents = await sut.Students.ToListAsync();
        allStudents.Should().HaveCount(2, "Es sollten nur 2 Entities existieren");
        
        allStudents.Should().Contain(s => s.Id == "STU-MERGE-001" && s.Name == "Updated Student 1");
        allStudents.Should().Contain(s => s.Id == "STU-MERGE-002" && s.Name == "Updated Student 2");
    }

    [Fact]
    public async Task MergeAsync_Works_With_Different_Entity_Types()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();
        var auto = new Auto
        {
            Id = "AUT-MERGE-001",
            Hersteller = "VW",
            Modell = "Golf",
            Baujahr = 2020,
            AnzahlTueren = 5,
            HatHybridantrieb = false
        };

        // Act - Erstes Hinzufügen
        var added = await sut.MergeAsync(auto);
        await sut.SaveChangesAsync();

        // Aktualisierte Version
        var updatedAuto = new Auto
        {
            Id = auto.Id,
            Hersteller = "Volkswagen",
            Modell = "Golf GTI",
            Baujahr = 2021,
            AnzahlTueren = 3,
            HatHybridantrieb = true
        };

        var merged = await sut.MergeAsync(updatedAuto);
        await sut.SaveChangesAsync();

        // Assert
        added.Should().BeTrue("Auto sollte hinzugefügt werden");
        merged.Should().BeFalse("Auto sollte aktualisiert werden");

        var saved = await sut.Autos.FirstOrDefaultAsync(a => a.Id == auto.Id);
        saved.Should().NotBeNull();
        saved!.Hersteller.Should().Be("Volkswagen");
        saved.Modell.Should().Be("Golf GTI");
        saved.Baujahr.Should().Be(2021);
        saved.AnzahlTueren.Should().Be(3);
        saved.HatHybridantrieb.Should().BeTrue();
    }

    [Fact]
    public async Task MergeAsync_Throws_ArgumentNullException_For_Null_Entity()
    {
        // Arrange
        using var sut = CreateInMemoryDbContext();

        // Act
        var act = async () => await sut.MergeAsync<Student>(null!);

        // Assert
        await act.Should().ThrowAsync<ArgumentNullException>()
            .WithParameterName("entity");
    }
}
