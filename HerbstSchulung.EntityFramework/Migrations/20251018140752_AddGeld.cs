using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerbstSchulung.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddGeld : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.ExecuteSqlFromResource(migrationBuilder, "Migrations/20251018140752_AddGeld_Up.sql");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.ExecuteSqlFromResource(migrationBuilder, "Migrations/20251018140752_AddGeld_Down.sql");
        }
    }
}
