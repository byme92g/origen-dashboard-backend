using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrigenDashboard.Migrations
{
    /// <inheritdoc />
    public partial class AddCaja : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Cantidad",
                table: "PaqueteProductos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.AlterColumn<int>(
                name: "Cantidad",
                table: "Ingresos",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 1);

            migrationBuilder.CreateTable(
                name: "CajaAperturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AbiertaEn = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    MontoInicial = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    Responsables = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CerradaEn = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    TotalIngresos = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    TotalEgresos = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    SaldoFinal = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    Observaciones = table.Column<string>(type: "longtext", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CajaAperturas", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CajaAperturas");

            migrationBuilder.AlterColumn<int>(
                name: "Cantidad",
                table: "PaqueteProductos",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Cantidad",
                table: "Ingresos",
                type: "int",
                nullable: false,
                defaultValue: 1,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
