using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenteMcService.Migrations
{
    /// <inheritdoc />
    public partial class RecreateDatabaseNewUsername : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 6, 20, 21, 23, 16, 529, DateTimeKind.Utc).AddTicks(7979));

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2025, 6, 21, 21, 23, 16, 529, DateTimeKind.Utc).AddTicks(7986));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 1,
                column: "Date",
                value: new DateTime(2025, 6, 20, 21, 20, 8, 344, DateTimeKind.Utc).AddTicks(2894));

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 2,
                column: "Date",
                value: new DateTime(2025, 6, 21, 21, 20, 8, 344, DateTimeKind.Utc).AddTicks(2900));
        }
    }
}
