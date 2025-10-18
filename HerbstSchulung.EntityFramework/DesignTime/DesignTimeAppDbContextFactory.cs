using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace HerbstSchulung.EntityFramework.DesignTime;

/// <summary>
/// Design-Time-Factory für EF Core Migrations.
/// Wird von der dotnet CLI verwendet, wenn kein Startprojekt bzw. keine Konfiguration verfügbar ist.
/// </summary>
public class DesignTimeAppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
        
        optionsBuilder.UseSqlServer("Server=localhost;Database=HerbstSchulung;Integrated Security=True;MultipleActiveResultSets=True;ConnectRetryCount=3;TrustServerCertificate=True");

        return new AppDbContext(optionsBuilder.Options);
    }
}
