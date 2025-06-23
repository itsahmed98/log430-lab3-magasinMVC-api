using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VenteMcService.Migrations
{
    /// <inheritdoc />
    public partial class AjoutClientEtIsEnLigneDansVente2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 6, 21, 19, 28, 5, 188, DateTimeKind.Utc).AddTicks(5261));

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2025, 6, 22, 19, 28, 5, 188, DateTimeKind.Utc).AddTicks(5267));

            migrationBuilder.InsertData(
                table: "Ventes",
                columns: new[] { "VenteId", "ClientId", "Date", "IsEnLigne", "MagasinId" },
                values: new object[,]
                {
                    { 3, 2, new DateTime(2025, 6, 20, 19, 28, 5, 188, DateTimeKind.Utc).AddTicks(5269), true, null },
                    { 4, null, new DateTime(2025, 6, 18, 19, 28, 5, 188, DateTimeKind.Utc).AddTicks(5270), false, 1 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 4);

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 6, 21, 19, 24, 17, 624, DateTimeKind.Utc).AddTicks(1233));

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2025, 6, 22, 19, 24, 17, 624, DateTimeKind.Utc).AddTicks(1243));
        }
    }
}
