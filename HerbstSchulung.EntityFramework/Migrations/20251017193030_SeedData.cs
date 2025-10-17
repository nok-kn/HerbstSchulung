using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HerbstSchulung.EntityFramework.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Fahrzeuge",
                columns: new[] { "Id", "Baujahr", "CreatedUtc", "Hersteller", "Modell" },
                values: new object[,]
                {
                    { "AUT-1234ABCD", 2020, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "VW", "Golf" },
                    { "LKW-9876WXYZ", 2018, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "MAN", "TGX" }
                });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "Art", "CreatedUtc", "Name", "Nationality", "School" },
                values: new object[] { "STD-ABCD2345", 0, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Max Mustermann", "DE", "HTL Linz" });

            migrationBuilder.InsertData(
                table: "Persons",
                columns: new[] { "Id", "Art", "CreatedUtc", "Name", "Nationality", "Subject" },
                values: new object[] { "TCR-EFGH6789", 1, new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Erika Musterfrau", "DE", "Mathematik" });

            migrationBuilder.InsertData(
                table: "Autos",
                columns: new[] { "Id", "AnzahlTueren", "HatHybridantrieb" },
                values: new object[] { "AUT-1234ABCD", 5, true });

            migrationBuilder.InsertData(
                table: "Lastkraftwagen",
                columns: new[] { "Id", "Achsen", "ZuladungInTonnen" },
                values: new object[] { "LKW-9876WXYZ", 3, 18.5 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Autos",
                keyColumn: "Id",
                keyValue: "AUT-1234ABCD");

            migrationBuilder.DeleteData(
                table: "Lastkraftwagen",
                keyColumn: "Id",
                keyValue: "LKW-9876WXYZ");

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: "STD-ABCD2345");

            migrationBuilder.DeleteData(
                table: "Persons",
                keyColumn: "Id",
                keyValue: "TCR-EFGH6789");

            migrationBuilder.DeleteData(
                table: "Fahrzeuge",
                keyColumn: "Id",
                keyValue: "AUT-1234ABCD");

            migrationBuilder.DeleteData(
                table: "Fahrzeuge",
                keyColumn: "Id",
                keyValue: "LKW-9876WXYZ");
        }
    }
}
