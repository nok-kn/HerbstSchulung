// NuGet: Microsoft.Data.SqlClient, Microsoft.EntityFrameworkCore.SqlServer,
// Microsoft.EntityFrameworkCore.Design (zum Migrieren), Respawn

using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Respawn;
using Respawn.Graph;
using Xunit;

namespace HerbstSchulung.EntityFramework.Tests;

public class DbFixture : IAsyncLifetime
{
    public SqlConnection Connection { get; }

    private Respawner? _respawner;
    
    public DbFixture()
    {
        var connectionString = Arrange.CetConnectionString();
        Connection = new SqlConnection(connectionString);
    }

    public async Task InitializeAsync()
    {

        // DB erstellen + migrieren (einmal je Testlauf)
        await using (var ctx = Arrange.CreateDbContext(false))
        {
            await ctx.Database.MigrateAsync();
            await SeedAsync(ctx);
        }

        await Connection.OpenAsync();

        // Respawn vorbereiten (nur relevante Schemas)
        _respawner = await Respawner.CreateAsync(Connection, new RespawnerOptions
        {
            SchemasToInclude = new[] { "dbo" },
            
            TablesToIgnore = new[]
            {
                new Table("__EFMigrationsHistory"),
                new Table("Laender"),
            },
            DbAdapter = DbAdapter.SqlServer
        });
    }

    public async Task ResetAsync() => await _respawner!.ResetAsync(Connection);

    public async Task DisposeAsync()
    {        
        await Connection.DisposeAsync();
    }

    public AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(Connection)
            .Options);

    private static Task SeedAsync(AppDbContext ctx)
    {
        return Task.CompletedTask; 
    }
}

// hier definieren wir die Tests Collection, für die der DbFixture genutzt wird
[CollectionDefinition("db")]
public class DbCollection : ICollectionFixture<DbFixture> { }