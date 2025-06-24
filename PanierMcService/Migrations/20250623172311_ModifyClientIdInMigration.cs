using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PanierMcService.Migrations
{
    /// <inheritdoc />
    public partial class ModifyClientIdInMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Paniers",
                keyColumn: "PanierId",
                keyValue: 1,
                column: "ClientId",
                value: 2);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Paniers",
                keyColumn: "PanierId",
                keyValue: 1,
                column: "ClientId",
                value: 100);
        }
    }
}
