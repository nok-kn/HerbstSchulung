using FluentAssertions;
using HerbstSchulung.EntityFramework.DataModel;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace HerbstSchulung.EntityFramework.Tests;

/// <summary>
/// Demonstriert verschiedene Bulk-Operationen mit EF Core.
/// </summary>
[Collection("db")]
public class BulkOperationsTests : IAsyncLifetime
{
    private readonly DbFixture _dbFixture;
    private readonly ITestOutputHelper _output;

    public BulkOperationsTests(DbFixture dbFixture, ITestOutputHelper output)
    {
        _dbFixture = dbFixture;
        _output = output;
        Arrange.SetTestOutputHelper(output);
    }

    [Fact]
    public async Task BulkInsert_BestPractice_Single_SaveChanges()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var nodes = GenerateNodes(100);

        // Schlecht:
        //foreach (var node in nodes)
        //{
        //    sut.Nodes.Add(node);
        //    await sut.SaveChangesAsync(); 
        //}

        // Act - GUT: Einmal SaveChanges
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        sut.Nodes.AddRange(nodes);  
        await sut.SaveChangesAsync(); // 1 DB-Roundtrip mit Batching
        stopwatch.Stop();
        _output.WriteLine($"Single SaveChanges: {stopwatch.ElapsedMilliseconds}ms für {nodes.Count} Einträge");

        // Assert
        var count = await sut.Nodes.CountAsync();
        count.Should().Be(100);
    }

    /// <summary>
    /// ExecuteUpdate für Updates ohne Entity-Tracking
    /// Extrem performant, da keine Entities geladen werden müssen.
    /// </summary>
    [Fact]
    public async Task BulkUpdate_ExecuteUpdate_Without_Loading_Entities()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var nodes = GenerateNodes(50);
        sut.Nodes.AddRange(nodes);
        await sut.SaveChangesAsync();
        sut.ChangeTracker.Clear();

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var affectedRows = await sut.Nodes
            .Where(n => n.Name.StartsWith("Node"))
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(n => n.Name, n => n.Name + " - Bulk Updated")); //  Direkt in SQL
        stopwatch.Stop();
        _output.WriteLine($"ExecuteUpdate: {stopwatch.ElapsedMilliseconds}ms für {affectedRows} Updates");

        // Assert
        affectedRows.Should().Be(50);
        var updatedNodes = await sut.Nodes.ToListAsync();
        updatedNodes.Should().OnlyContain(n => n.Name.EndsWith(" - Bulk Updated"));
    }
   
    /// <summary>
    /// ExecuteDelete für  Löschen ohne Entity Tracking.
    /// Extrem performant, da keine Entities geladen werden müssen.
    /// VORSICHT: Umgeht Change Tracker und Cascade Delete Logik!
    /// </summary>
    [Fact]
    public async Task BulkDelete_ExecuteDelete_Without_Loading_Entities()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var nodes = GenerateNodes(50);
        sut.Nodes.AddRange(nodes);
        await sut.SaveChangesAsync();
        sut.ChangeTracker.Clear();

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var deletedRows = await sut.Nodes
            .Where(n => n.Name.StartsWith("Node"))
            .ExecuteDeleteAsync(); //  Direkt in SQL DELETE
        stopwatch.Stop();
        _output.WriteLine($"ExecuteDelete: {stopwatch.ElapsedMilliseconds}ms für {deletedRows} Deletes");

        // Assert
        deletedRows.Should().Be(50);
        var count = await sut.Nodes.CountAsync();
        count.Should().Be(0);
    }


    /// <summary>
    /// Demonstriert eine komplexe Bulk-Operation: Insert, Update und Delete in einer Transaktion.
    /// WICHTIG: Bei aktivierter Retry-Strategie muss CreateExecutionStrategy() verwendet werden!
    /// </summary>
    [Fact]
    public async Task Complex_Bulk_Operation_Insert_Update_Delete_In_Transaction()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var existingNodes = GenerateNodes(50, "Existing");
        sut.Nodes.AddRange(existingNodes);
        await sut.SaveChangesAsync();
        sut.ChangeTracker.Clear();

        // Act - Mit ExecutionStrategy für Retry-Unterstützung
        // Bei EnableRetryOnFailure muss CreateExecutionStrategy() verwendet werden!
        var strategy = sut.Database.CreateExecutionStrategy();

        // Transaktion muss innerhalb der ExecutionStrategy sein, da bei Retry Logik verwenden
        async Task PerformManyOperationsInOneTransaction()
        {
            await using var transaction = await sut.Database.BeginTransactionAsync();
            // 1. Neue Nodes einfügen
            var newNodes = GenerateNodes(25, "New");
            sut.Nodes.AddRange(newNodes);

            // 2. Bestehende Nodes aktualisieren
            var nodesToUpdate = await sut.Nodes.Where(n => n.Name.StartsWith("Existing") && n.Name.Contains("1"))
                .ToListAsync();

            foreach (var node in nodesToUpdate)
            {
                node.Name = node.Name.Replace("Existing", "Updated");
            }

            // 3. Einige Nodes löschen
            var nodesToDelete = await sut.Nodes.Where(n => n.Name.StartsWith("Existing") && n.Name.Contains("2"))
                .ToListAsync();

            sut.Nodes.RemoveRange(nodesToDelete);

            // 4. Alles auf einmal speichern
            await sut.SaveChangesAsync();
            await transaction.CommitAsync();
        }

        await strategy.ExecuteAsync(PerformManyOperationsInOneTransaction);

        // Assert
        var totalCount = await sut.Nodes.CountAsync();
        var updatedCount = await sut.Nodes.CountAsync(n => n.Name.StartsWith("Updated"));
        var newCount = await sut.Nodes.CountAsync(n => n.Name.StartsWith("New"));

        _output.WriteLine($"Total: {totalCount}, Updated: {updatedCount}, New: {newCount}");
        
        totalCount.Should().BeGreaterThan(0);
        updatedCount.Should().BeGreaterThan(0);
        newCount.Should().Be(25);
    }

    // ansonsten: SqlBulkCopy oder Drittanbieter-Bibliotheken wie EFCore.BulkExtensions (kostenpflichtig)
    
    private static List<Node> GenerateNodes(int count, string prefix = "Node")
    {
        var nodes = new List<Node>();
        for (int i = 0; i < count; i++)
        {
            nodes.Add(new Node { Name = $"{prefix} {i:D4}" });
        }
        return nodes;
    }

    public Task InitializeAsync() => Task.CompletedTask;

    public Task DisposeAsync() => _dbFixture.ResetAsync();

}
