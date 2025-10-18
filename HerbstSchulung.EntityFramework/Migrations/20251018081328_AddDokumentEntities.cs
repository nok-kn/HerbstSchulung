using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HerbstSchulung.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddDokumentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.ExecuteSqlFromResource(migrationBuilder, "Migrations/20251018081328_AddDokumentEntities_Up.sql");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            MigrationHelper.ExecuteSqlFromResource(migrationBuilder, "Migrations/20251018081328_AddDokumentEntities_Down.sql");
        }
    }
}
