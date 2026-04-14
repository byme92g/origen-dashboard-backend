using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrigenDashboard.Migrations
{
    /// <inheritdoc />
    public partial class AddCantidadToIngreso : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Cantidad",
                table: "Ingresos",
                type: "int",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AddColumn<decimal>(
                name: "ComisionPct",
                table: "Paquetes",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cantidad",
                table: "Ingresos");

            migrationBuilder.DropColumn(
                name: "ComisionPct",
                table: "Paquetes");
        }
    }
}
