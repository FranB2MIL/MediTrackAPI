using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixMeasurementColumnTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "Measurements",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "Size",
                table: "Measurements",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "IMC",
                table: "Measurements",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<decimal>(
                name: "Height",
                table: "Measurements",
                type: "REAL",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "TEXT");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Weight",
                table: "Measurements",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "Size",
                table: "Measurements",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "IMC",
                table: "Measurements",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "REAL");

            migrationBuilder.AlterColumn<decimal>(
                name: "Height",
                table: "Measurements",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "REAL");
        }
    }
}
