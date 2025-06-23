using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenteMcService.Migrations
{
    /// <inheritdoc />
    public partial class RefaireBasedeDonnees : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 6, 21, 19, 34, 39, 887, DateTimeKind.Utc).AddTicks(659));

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2025, 6, 22, 19, 34, 39, 887, DateTimeKind.Utc).AddTicks(666));

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 3,
                column: "Date",
                value: new DateTime(2025, 6, 20, 19, 34, 39, 887, DateTimeKind.Utc).AddTicks(667));

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 4,
                column: "Date",
                value: new DateTime(2025, 6, 18, 19, 34, 39, 887, DateTimeKind.Utc).AddTicks(669));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 3,
                column: "Date",
                value: new DateTime(2025, 6, 20, 19, 28, 5, 188, DateTimeKind.Utc).AddTicks(5269));

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 4,
                column: "Date",
                value: new DateTime(2025, 6, 18, 19, 28, 5, 188, DateTimeKind.Utc).AddTicks(5270));
        }
    }
}
