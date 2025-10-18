// NuGet: Microsoft.Data.SqlClient, Microsoft.EntityFrameworkCore.SqlServer,
// Microsoft.EntityFrameworkCore.Design (zum Migrieren), Respawn
using HerbstSchulung.EntityFramework;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Respawn;
using Respawn.Graph;
using Xunit;

public class DbFixture : IAsyncLifetime
{
    public SqlConnection Connection { get; private set; } = default!;
    
    private Respawner _respawner = default!;
    
    private  readonly string _connectionString;

    public DbFixture()
    {
        // Konfiguration aus appsettings.json laden
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .Build();

        _connectionString = configuration.GetConnectionString("DefaultConnection");
    }

    public async Task InitializeAsync()
    {
        Connection = new SqlConnection(_connectionString);
        await Connection.OpenAsync();

        // DB erstellen + migrieren (einmal je Testlauf)
        await using (var ctx = CreateContext())
        {
            await ctx.Database.MigrateAsync();
            await SeedMinimalAsync(ctx);
        }

        // Respawn vorbereiten (nur relevante Schemata)
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

    public async Task ResetAsync() => await _respawner.ResetAsync(Connection);

    public async Task DisposeAsync()
    {        
        await Connection.DisposeAsync();
    }

    public AppDbContext CreateContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(Connection)
            .Options);

    private static Task SeedMinimalAsync(AppDbContext ctx)
    {
        return Task.CompletedTask; 
    }
}

// 
[CollectionDefinition("db")]
public class DbCollection : ICollectionFixture<DbFixture> { }
