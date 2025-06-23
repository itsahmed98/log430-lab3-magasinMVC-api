using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VenteMcService.Migrations
{
    /// <inheritdoc />
    public partial class AjoutClientEtIsEnLigneDansVente : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "MagasinId",
                table: "Ventes",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "ClientId",
                table: "Ventes",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEnLigne",
                table: "Ventes",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 1,
                columns: new[] { "ClientId", "Date", "IsEnLigne" },
                values: new object[] { null, new DateTime(2025, 6, 21, 19, 24, 17, 624, DateTimeKind.Utc).AddTicks(1233), false });

            migrationBuilder.UpdateData(
                table: "Ventes",
                keyColumn: "VenteId",
                keyValue: 2,
                columns: new[] { "ClientId", "Date", "IsEnLigne" },
                values: new object[] { null, new DateTime(2025, 6, 22, 19, 24, 17, 624, DateTimeKind.Utc).AddTicks(1243), false });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ClientId",
                table: "Ventes");

            migrationBuilder.DropColumn(
                name: "IsEnLigne",
                table: "Ventes");

            migrationBuilder.AlterColumn<int>(
                name: "MagasinId",
                table: "Ventes",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

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
    }
}
