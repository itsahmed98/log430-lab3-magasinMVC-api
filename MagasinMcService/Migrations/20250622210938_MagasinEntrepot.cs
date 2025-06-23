using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MagasinMcService.Migrations
{
    /// <inheritdoc />
    public partial class MagasinEntrepot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Magasins",
                keyColumn: "MagasinId",
                keyValue: 1,
                columns: new[] { "Adresse", "Nom" },
                values: new object[] { "123 Rue entrepot", "Entrepot Central" });

            migrationBuilder.UpdateData(
                table: "Magasins",
                keyColumn: "MagasinId",
                keyValue: 2,
                columns: new[] { "Adresse", "Nom" },
                values: new object[] { "123 Rue Principale", "Magasin A" });

            migrationBuilder.UpdateData(
                table: "Magasins",
                keyColumn: "MagasinId",
                keyValue: 3,
                columns: new[] { "Adresse", "Nom" },
                values: new object[] { "456 Avenue Centrale", "Magasin B" });

            migrationBuilder.UpdateData(
                table: "Magasins",
                keyColumn: "MagasinId",
                keyValue: 4,
                columns: new[] { "Adresse", "Nom" },
                values: new object[] { "789 Boulevard Sud", "Magasin C" });

            migrationBuilder.InsertData(
                table: "Magasins",
                columns: new[] { "MagasinId", "Adresse", "Nom" },
                values: new object[] { 5, "321 Rue Nord", "Magasin D" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Magasins",
                keyColumn: "MagasinId",
                keyValue: 5);

            migrationBuilder.UpdateData(
                table: "Magasins",
                keyColumn: "MagasinId",
                keyValue: 1,
                columns: new[] { "Adresse", "Nom" },
                values: new object[] { "123 Rue Principale", "Magasin A" });

            migrationBuilder.UpdateData(
                table: "Magasins",
                keyColumn: "MagasinId",
                keyValue: 2,
                columns: new[] { "Adresse", "Nom" },
                values: new object[] { "456 Avenue Centrale", "Magasin B" });

            migrationBuilder.UpdateData(
                table: "Magasins",
                keyColumn: "MagasinId",
                keyValue: 3,
                columns: new[] { "Adresse", "Nom" },
                values: new object[] { "789 Boulevard Sud", "Magasin C" });

            migrationBuilder.UpdateData(
                table: "Magasins",
                keyColumn: "MagasinId",
                keyValue: 4,
                columns: new[] { "Adresse", "Nom" },
                values: new object[] { "321 Rue Nord", "Magasin D" });
        }
    }
}
