using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace StockMcService.Migrations
{
    /// <inheritdoc />
    public partial class CorrigerIdMagasinStockService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 2, 3 });

            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 3, 2 });

            migrationBuilder.UpdateData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 1, 1 },
                column: "Quantite",
                value: 400);

            migrationBuilder.UpdateData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 1, 2 },
                column: "Quantite",
                value: 400);

            migrationBuilder.UpdateData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 2, 1 },
                column: "Quantite",
                value: 100);

            migrationBuilder.InsertData(
                table: "StockItems",
                columns: new[] { "MagasinId", "ProduitId", "Quantite" },
                values: new object[,]
                {
                    { 1, 3, 400 },
                    { 2, 2, 80 },
                    { 3, 1, 60 },
                    { 3, 3, 90 },
                    { 4, 2, 200 },
                    { 5, 1, 310 },
                    { 5, 2, 100 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 1, 3 });

            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 2, 2 });

            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 3, 1 });

            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 3, 3 });

            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 4, 2 });

            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 5, 1 });

            migrationBuilder.DeleteData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 5, 2 });

            migrationBuilder.UpdateData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 1, 1 },
                column: "Quantite",
                value: 100);

            migrationBuilder.UpdateData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 1, 2 },
                column: "Quantite",
                value: 80);

            migrationBuilder.UpdateData(
                table: "StockItems",
                keyColumns: new[] { "MagasinId", "ProduitId" },
                keyValues: new object[] { 2, 1 },
                column: "Quantite",
                value: 60);

            migrationBuilder.InsertData(
                table: "StockItems",
                columns: new[] { "MagasinId", "ProduitId", "Quantite" },
                values: new object[,]
                {
                    { 0, 1, 400 },
                    { 0, 2, 400 },
                    { 0, 3, 400 },
                    { 2, 3, 90 },
                    { 3, 2, 30 }
                });
        }
    }
}
