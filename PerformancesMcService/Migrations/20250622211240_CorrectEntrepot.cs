using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PerformancesMcService.Migrations
{
    /// <inheritdoc />
    public partial class CorrectEntrepot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 1,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Utc), 2 });

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 2,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Utc), 3 });

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 3,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 21, 0, 0, 0, 0, DateTimeKind.Utc), 2 });

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 4,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 21, 0, 0, 0, 0, DateTimeKind.Utc), 3 });

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 5,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 22, 0, 0, 0, 0, DateTimeKind.Utc), 2 });

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 6,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 22, 0, 0, 0, 0, DateTimeKind.Utc), 3 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 1,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 16, 0, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 2,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 16, 0, 0, 0, 0, DateTimeKind.Utc), 2 });

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 3,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 17, 0, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 4,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 17, 0, 0, 0, 0, DateTimeKind.Utc), 2 });

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 5,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 18, 0, 0, 0, 0, DateTimeKind.Utc), 1 });

            migrationBuilder.UpdateData(
                table: "Performances",
                keyColumn: "PerformanceId",
                keyValue: 6,
                columns: new[] { "Date", "MagasinId" },
                values: new object[] { new DateTime(2025, 6, 18, 0, 0, 0, 0, DateTimeKind.Utc), 2 });
        }
    }
}
