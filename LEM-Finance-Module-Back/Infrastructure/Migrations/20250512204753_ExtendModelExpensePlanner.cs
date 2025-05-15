using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ExtendModelExpensePlanner : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "ExpensePlanner",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "GrossPricePLN",
                table: "ExpensePlanner",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "NetPricePLN",
                table: "ExpensePlanner",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxPLN",
                table: "ExpensePlanner",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExchangeRate",
                table: "ExpensePlanner");

            migrationBuilder.DropColumn(
                name: "GrossPricePLN",
                table: "ExpensePlanner");

            migrationBuilder.DropColumn(
                name: "NetPricePLN",
                table: "ExpensePlanner");

            migrationBuilder.DropColumn(
                name: "TaxPLN",
                table: "ExpensePlanner");
        }
    }
}
