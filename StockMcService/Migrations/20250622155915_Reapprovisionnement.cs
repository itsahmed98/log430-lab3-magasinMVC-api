using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StockMcService.Migrations
{
    /// <inheritdoc />
    public partial class Reapprovisionnement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Demandes",
                columns: table => new
                {
                    DemandeId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MagasinId = table.Column<int>(type: "integer", nullable: false),
                    ProduitId = table.Column<int>(type: "integer", nullable: false),
                    QuantiteDemandee = table.Column<int>(type: "integer", nullable: false),
                    Statut = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Demandes", x => x.DemandeId);
                });

            migrationBuilder.InsertData(
                table: "StockItems",
                columns: new[] { "MagasinId", "ProduitId", "Quantite" },
                values: new object[,]
                {
                    { 0, 1, 400 },
                    { 0, 2, 400 },
                    { 0, 3, 400 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Demandes");

            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 0, 1 });

            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 0, 2 });

            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 0, 3 });
        }
    }
}
