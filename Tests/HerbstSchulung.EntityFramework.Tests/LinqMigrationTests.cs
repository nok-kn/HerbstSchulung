using FluentAssertions;
using HerbstSchulung.EntityFramework.DataModel;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace HerbstSchulung.EntityFramework.Tests;

/// <summary>
/// Demonstriert h�ufige LINQ-Probleme bei der Migration von EF zu EF Core und deren L�sungen
/// </summary>
[Collection("db")]
public class LinqMigrationTests : IAsyncLifetime
{
    private readonly DbFixture _dbFixture;

    public LinqMigrationTests(DbFixture dbFixture, ITestOutputHelper output)
    {
        _dbFixture = dbFixture;
        Arrange.SetTestOutputHelper(output);
    }

    /// <summary>
    /// Problem: EF6 erlaubte komplexe GroupBy-Operationen, die in EF Core zu Exceptions f�hren k�nnen.
    /// EF Core �bersetzt GroupBy strikter und erfordert oft Client-Evaluierung.
    /// </summary>
    [Fact]
    public async Task GroupBy_Problem_Complex_Aggregation_Can_Fails_In_EF_Core()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        await ArrangePersonsWithNationalities(sut);

        var act = async () => await sut.Set<Person>()
            .GroupBy(p => p.Nationality)
            .Select(g => new
            {
                Nationality = g.Key,
                Count = g.Count(),
                Names = string.Join(", ", g.Select(p => p.Name).OrderBy(n => n)),
                UppercaseNames = string.Join(" | ", g.Select(p => p.Name.ToUpper())),
                StudentCount = g.OfType<Student>().Count(),
                TeacherCount = g.OfType<Teacher>().Count(),
                LongNameCount = g.Count(p => p.Name.Length > 10),
                FirstNames = g.Any() ? g.First().Name : "No Names",
                Summary = $"{g.Key}: {g.Count()} persons - " +
                          string.Join(", ", g.Select(p => p.Name.Substring(0, Math.Min(3, p.Name.Length))))
            })
            .ToListAsync();

        await act.Should().NotThrowAsync<InvalidOperationException>(); // funktioniert doch in EF Core 9!
    }

    // L�sung => EF Core 9 unterst�tzt komplexe GroupBy-Operationen besser.

    /// <summary>
    /// L�sung - Einfache Aggregationen in der Datenbank, komplexe Logik danach.
    /// Effizientere L�sung als vollst�ndige Client-Evaluierung.
    /// </summary>
    [Fact]
    public async Task GroupBy_Solution_Simple_Aggregation_Then_Materialize()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        await ArrangePersonsWithNationalities(sut);

        // Act - Nur einfache Aggregation in DB
        var dbResult = await sut.Set<Person>()
            .GroupBy(p => p.Nationality)
            .Select(g => new
            {
                Nationality = g.Key,
                Count = g.Count(),
                PersonIds = g.Select(p => p.Id).ToList()
            })
            .ToListAsync();

        // Komplexe Logik dann in-memory
        var result = dbResult.Select(x => new
        {
            x.Nationality,
            x.Count,
            // Zweite Query f�r Namen (wenn n�tig)
            Names = string.Join(", ", sut.Set<Person>()
                .Where(p => x.PersonIds.Contains(p.Id))
                .Select(p => p.Name)
                .OrderBy(n => n)
                .ToList())
        }).ToList();

        // Assert
        result.Should().HaveCount(3);
        var german = result.Single(x => x.Nationality == "DE");
        german.Count.Should().Be(2);
    }



    private async Task ArrangePersonsWithNationalities(AppDbContext ctx)
    {
        var students = new[]
        {
            new Student
            {
                Name = "Max Student",
                Nationality = "DE",
                School = "Gymnasium Munich"
            },
            new Student
            {
                Name = "Anna Student",
                Nationality = "AT",
                School = "HTL Vienna"
            }
        };

        var teachers = new[]
        {
            new Teacher
            {
                Name = "Peter Teacher",
                Nationality = "DE",
                Subject = "Mathematics"
            },
            new Teacher
            {
                Name = "Maria Teacher",
                Nationality = "CH",
                Subject = "Physics"
            }
        };

        ctx.Students.AddRange(students);
        ctx.Teachers.AddRange(teachers);
        await ctx.SaveChangesAsync();
    }


    // Wird vor jedem Test ausgef�hrt
    public Task InitializeAsync()
    {
        return _dbFixture.ResetAsync();
    }

    // Wird nach jedem Test ausgef�hrt
    public Task DisposeAsync()
    {
        return _dbFixture.ResetAsync();
    }
}
