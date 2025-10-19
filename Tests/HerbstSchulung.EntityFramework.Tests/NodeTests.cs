using FluentAssertions;
using HerbstSchulung.EntityFramework.DataModel;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace HerbstSchulung.EntityFramework.Tests;

[Collection("db")]
public class NodeTests : IAsyncLifetime
{
    private readonly DbFixture _dbFixture;
    private readonly ITestOutputHelper _output;

    public NodeTests(DbFixture dbFixture, ITestOutputHelper output)
    {
        _dbFixture = dbFixture;
        _output = output;
        
        // Setze den Test-Output-Helper für EF Core Logging in den Tests
        Arrange.SetTestOutputHelper(output);
    }

    [Fact]
    public async Task Node_Can_Be_Created_Without_Parent()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var rootNode = new Node
        {
            Name = "Root Node"
        };

        // Act
        sut.Nodes.Add(rootNode);
        await sut.SaveChangesAsync();

        // Assert
        var actual = await sut.Nodes.FirstOrDefaultAsync(n => n.Id == rootNode.Id);
        actual.Should().NotBeNull();
        actual!.Name.Should().Be("Root Node");
        actual.ParentId.Should().BeNull();
        actual.Parent.Should().BeNull();
    }

    [Fact]
    public async Task Node_Can_Have_Parent_And_Children()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var parent = new Node { Name = "Parent Node" };
        var child1 = new Node { Name = "Child 1" };
        var child2 = new Node { Name = "Child 2" };

        // Act
        sut.Nodes.Add(parent);
        child1.ParentId = parent.Id;
        child2.ParentId = parent.Id;
        sut.Nodes.Add(child1);
        sut.Nodes.Add(child2);
        await sut.SaveChangesAsync();

        // Assert
        sut.ChangeTracker.Clear();
        var actualParent = await sut.Nodes
            .Include(n => n.Children)
            .AsSplitQuery()                   // split sollte 2 Abfragen erstellen:
                                              // 1. Parent
                                              // 2. Children
                                              // und dann diese Ergebnisse internal zusammenführen   

            .FirstOrDefaultAsync(n => n.Id == parent.Id);

        actualParent.Should().NotBeNull();
        actualParent!.Children.Should().HaveCount(2);
        actualParent.Children.Should().Contain(c => c.Name == "Child 1");
        actualParent.Children.Should().Contain(c => c.Name == "Child 2");
    }

    [Fact]
    public async Task Node_Children_Can_Access_Parent()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var parent = new Node { Name = "Parent Node" };
        var child = new Node { Name = "Child Node" };

        sut.Nodes.Add(parent);
        await sut.SaveChangesAsync();

        child.ParentId = parent.Id;
        sut.Nodes.Add(child);
        await sut.SaveChangesAsync();

        // Act
        sut.ChangeTracker.Clear();
        var actualChild = await sut.Nodes
            .Include(n => n.Parent)
            .FirstOrDefaultAsync(n => n.Id == child.Id);

        // Assert
        actualChild.Should().NotBeNull();
        actualChild!.Parent.Should().NotBeNull();
        actualChild.Parent!.Name.Should().Be("Parent Node");
        actualChild.ParentId.Should().Be(parent.Id);
    }

    [Fact]
    public async Task Node_Can_Create_Multi_Level_Hierarchy()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var root = new Node { Name = "Root" };
        var level1 = new Node { Name = "Level 1" };
        var level2 = new Node { Name = "Level 2" };

        // Act
        sut.Nodes.Add(root);
        await sut.SaveChangesAsync();

        level1.ParentId = root.Id;
        sut.Nodes.Add(level1);
        await sut.SaveChangesAsync();

        level2.ParentId = level1.Id;
        sut.Nodes.Add(level2);
        await sut.SaveChangesAsync();

        // Assert
        sut.ChangeTracker.Clear();
        var actualLevel2 = await sut.Nodes
            .Include(n => n.Parent)
                .ThenInclude(p => p!.Parent)
            .FirstOrDefaultAsync(n => n.Id == level2.Id);

        actualLevel2.Should().NotBeNull();
        actualLevel2!.Name.Should().Be("Level 2");
        actualLevel2.Parent.Should().NotBeNull();
        actualLevel2.Parent!.Name.Should().Be("Level 1");
        actualLevel2.Parent.Parent.Should().NotBeNull();
        actualLevel2.Parent.Parent!.Name.Should().Be("Root");
    }

    [Fact]
    public async Task Node_Can_Update_Parent()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var parent1 = new Node { Name = "Parent 1" };
        var parent2 = new Node { Name = "Parent 2" };
        var child = new Node { Name = "Child" };

        sut.Nodes.Add(parent1);
        sut.Nodes.Add(parent2);
        await sut.SaveChangesAsync();

        child.ParentId = parent1.Id;
        sut.Nodes.Add(child);
        await sut.SaveChangesAsync();

        // Act - Change parent
        child.ParentId = parent2.Id;
        await sut.SaveChangesAsync();

        // Assert
        sut.ChangeTracker.Clear();
        var actual = await sut.Nodes
            .Include(n => n.Parent)
            .FirstOrDefaultAsync(n => n.Id == child.Id);

        actual.Should().NotBeNull();
        actual!.ParentId.Should().Be(parent2.Id);
        actual.Parent!.Name.Should().Be("Parent 2");
    }

    [Fact]
    public async Task Node_Can_Query_All_Root_Nodes()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var root1 = new Node { Name = "Root 1" };
        var root2 = new Node { Name = "Root 2" };
        var child = new Node { Name = "Child" };

        sut.Nodes.Add(root1);
        sut.Nodes.Add(root2);
        await sut.SaveChangesAsync();

        child.ParentId = root1.Id;
        sut.Nodes.Add(child);
        await sut.SaveChangesAsync();

        // Act
        var rootNodes = await sut.Nodes
            .Where(n => n.ParentId == null)
            .ToListAsync();

        // Assert
        rootNodes.Should().HaveCount(2);
        rootNodes.Should().Contain(n => n.Name == "Root 1");
        rootNodes.Should().Contain(n => n.Name == "Root 2");
    }

    // Wird vor jedem Test ausgeführt
    public Task InitializeAsync()
    {
        return _dbFixture.ResetAsync();
    }

    // Wird nach jedem Test ausgeführt - DB zurücksetzen
    public Task DisposeAsync()
    {
        return _dbFixture.ResetAsync();
    }
}
