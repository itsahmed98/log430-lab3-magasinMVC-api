using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenteMcService.Migrations
{
    /// <inheritdoc />
    public partial class CorrigerIdMagasinVenteService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 1,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 20, 21, 20, 8, 344, DateTimeKind.Utc).AddTicks(2894), 2 });

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 2,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 21, 21, 20, 8, 344, DateTimeKind.Utc).AddTicks(2900), 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 1,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 16, 18, 25, 38, 75, DateTimeKind.Utc).AddTicks(9750), 1 });

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 2,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 17, 18, 25, 38, 75, DateTimeKind.Utc).AddTicks(9757), 2 });
        }
    }
}
