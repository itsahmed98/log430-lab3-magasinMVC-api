using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MagasinMcService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Magasins",
                columns: table => new
                {
                    MagasinId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Adresse = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Magasins", x => x.MagasinId);
                });

            migrationBuilder.InsertData(
                table: "Magasins",
                columns: new[] { "MagasinId", "Adresse", "Nom" },
                values: new object[,]
                {
                    { 1, "123 Rue Principale", "Magasin A" },
                    { 2, "456 Avenue Centrale", "Magasin B" },
                    { 3, "789 Boulevard Sud", "Magasin C" },
                    { 4, "321 Rue Nord", "Magasin D" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Magasins");
        }
    }
}
