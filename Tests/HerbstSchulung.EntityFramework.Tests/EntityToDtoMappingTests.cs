using FluentAssertions;
using HerbstSchulung.EntityFramework.DataModel;
using HerbstSchulung.EntityFramework.Tests.DTOs;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Xunit.Abstractions;

namespace HerbstSchulung.EntityFramework.Tests;

/// <summary>
/// Demonstriert das Mapping von Entity zu DTO mit manueller Mapping-Logik und Mapster.
/// </summary>
[Collection("db")]
public class EntityToDtoMappingTests : IAsyncLifetime
{
    private readonly DbFixture _dbFixture;

    public EntityToDtoMappingTests(DbFixture dbFixture, ITestOutputHelper output)
    {
        _dbFixture = dbFixture;
        Arrange.SetTestOutputHelper(output);
    }

    // 1. Möglichkeit:  manuelles Mapping Entity -> DTO 
    // Volle Kontrolle aber mehr boilerplate, Wartungsaufwand 
    [Fact]
    public async Task Node_Can_Be_Mapped_To_Dto_Manually_After_Include()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var parent = await ArrangeParentWithChildren(sut);
        sut.ChangeTracker.Clear();

        // Act - Entity laden mit Include und MANUELL zu DTO mappen
        var parentEntity = await sut.Nodes
            .Include(n => n.Children)
            .AsSplitQuery() // Split Query für Include ist OK
            .FirstOrDefaultAsync(n => n.Id == parent.Id);

        // Manuelles Mapping: Entity -> DTO (mit Record Constructor)
        var parentDto = new NodeWithChildrenDto(
            Id: parentEntity!.Id,
            Name: parentEntity.Name,
            ParentId: parentEntity.ParentId,
            Children: parentEntity.Children.Select(c => new NodeWithChildrenDto(
                Id: c.Id,
                Name: c.Name,
                ParentId: c.ParentId,
                Children: [] // Keine weiteren Children laden
            )).ToList()
        );

        // Assert
        AssertParentWithChildren(parentDto, parent.Id);
    }

    // 2. Möglichkeit:  automatisches Mapping Entity -> DTO 
    // Weniger Kontrolleme, aber weniger Code und Wartungsaufwand
    [Fact]
    public async Task Node_Can_Be_Mapped_To_Dto_Using_Mapster_After_Include()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var parent = await ArrangeParentWithChildren(sut);
        sut.ChangeTracker.Clear();

        // Act - Entity laden mit Include und mit MAPSTER zu DTO mappen
        var parentEntity = await sut.Nodes
            .Include(n => n.Children)
            .AsSplitQuery() // Split Query für Include ist OK
            .FirstOrDefaultAsync(n => n.Id == parent.Id);

        // Mapster Mapping: Entity -> DTO
        // Mapster erkennt automatisch die Property-Namen und mappt sie
        var parentDto = parentEntity!.Adapt<NodeWithChildrenDto>();

        // Assert
        AssertParentWithChildren(parentDto, parent.Id);
    }

    // 3. Möglichkeit:  Projection with Select (Manual) -> DTO 
    // Beste Performance (minimale Daten) aber kann zur komplexen Queries führen, AsSplitQuery funktioniert NICHT
    [Fact]
    public async Task Node_Can_Be_Projected_To_Dto_Manually_With_Select()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var parent = await ArrangeParentWithChildren(sut);

        // Act - Direkte Projektion in der Datenbank mit Select
        //  AsSplitQuery() funktioniert NICHT mit Select-Projektionen!
        var parentDto = await sut.Nodes
            .Where(n => n.Id == parent.Id)
            .Select(n => new NodeWithChildrenDto(
                n.Id,
                n.Name,
                n.ParentId,
                n.Children.Select(c => new NodeWithChildrenDto(
                    c.Id,
                    c.Name,
                    c.ParentId,
                    new List<NodeWithChildrenDto>() // Keine weiteren Children
                )).ToList()
            ))
            .FirstOrDefaultAsync();

        // Assert
        parentDto.Should().NotBeNull();
        AssertParentWithChildren(parentDto!, parent.Id);
    }

    // 4. Möglichkeit: Projection with Mapster ProjectToType
    // Beste Performance (minimale Daten) und wenig Code durch automatische Projektion
    [Fact]
    public async Task Node_Can_Be_Projected_To_Dto_Using_Mapster_ProjectToType()
    {
        // Arrange
        await using var sut = Arrange.CreateDbContext(false);
        var parent = await ArrangeParentWithChildren(sut);

        // Mapster Konfiguration für maximale Tiefe bei rekursiven Dtos
        TypeAdapterConfig.GlobalSettings.Default.MaxDepth(2);

        // Act - Mapster ProjectToType erstellt automatisch die Select-Projektion
        var parentDto = await sut.Nodes
            .Where(n => n.Id == parent.Id)
            .ProjectToType<NodeWithChildrenDto>() // Mapster übersetzt in Select
            .FirstOrDefaultAsync();

        // Assert
        parentDto.Should().NotBeNull();
        AssertParentWithChildren(parentDto!, parent.Id);
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

    #region Arrange Methods

    private static async Task<Node> ArrangeParentWithChildren(AppDbContext sut)
    {
        var parent = new Node { Name = "Parent Node" };
        var child1 = new Node { Name = "Child 1" };
        var child2 = new Node { Name = "Child 2" };

        sut.Nodes.Add(parent);
        await sut.SaveChangesAsync();

        child1.ParentId = parent.Id;
        child2.ParentId = parent.Id;
        sut.Nodes.Add(child1);
        sut.Nodes.Add(child2);
        await sut.SaveChangesAsync();

        return parent;
    }

    private static void AssertParentWithChildren(NodeWithChildrenDto parentDto, string expectedParentId)
    {
        parentDto.Should().NotBeNull();
        parentDto.Name.Should().Be("Parent Node");
        parentDto.Children.Should().HaveCount(2);
        parentDto.Children.Should().Contain(c => c.Name == "Child 1");
        parentDto.Children.Should().Contain(c => c.Name == "Child 2");
        parentDto.Children.Should().AllSatisfy(c => c.ParentId.Should().Be(expectedParentId));
        parentDto.Children.Should().AllSatisfy(c => (c.Children.Count == 0).Should().BeTrue());
    }


    #endregion
}
