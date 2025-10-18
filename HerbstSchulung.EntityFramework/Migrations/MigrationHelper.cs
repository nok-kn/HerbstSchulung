using Microsoft.EntityFrameworkCore.Migrations;

namespace HerbstSchulung.EntityFramework.Migrations;

public static class MigrationHelper
{
    public static void ExecuteSqlFromResource(MigrationBuilder migrationBuilder, string resourcePath, bool suppressTransaction = false)
    {
        var asm = typeof(MigrationHelper).Assembly;

        // Resource name: "<AssemblyName>.<path with dots>"
        var normalized = resourcePath.Replace('/', '.').Replace('\\', '.');

        if (normalized.StartsWith('.'))
        {
            normalized = normalized.TrimStart('.');
        }
            
        var fullName = $"{asm.GetName().Name}.{normalized}";

        using var stream = asm.GetManifestResourceStream(fullName) ?? throw new FileNotFoundException($"Embedded SQL resource not found: {fullName}");
        using var reader = new StreamReader(stream);
        var sql = reader.ReadToEnd();
        migrationBuilder.Sql(sql, suppressTransaction: suppressTransaction);
    }
}
