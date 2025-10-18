using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HerbstSchulung.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class AddLaender : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Laender",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsoCode = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laender", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Laender",
                columns: new[] { "Id", "CreatedUtc", "IsoCode", "Name" },
                values: new object[,]
                {
                    { "LND-AT000001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "AT", "Österreich" },
                    { "LND-CH000001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "CH", "Schweiz" },
                    { "LND-DE000001", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "DE", "Deutschland" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Laender");
        }
    }
}
