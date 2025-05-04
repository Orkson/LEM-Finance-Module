using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessarytable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpensePlanner_Devices_DeviceId",
                table: "ExpensePlanner");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpensePlanner_Service_ServiceId",
                table: "ExpensePlanner");

            migrationBuilder.DropTable(
                name: "CalibrationCosts");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                table: "ExpensePlanner",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "ExpensePlanner",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpensePlanner_Devices_DeviceId",
                table: "ExpensePlanner",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpensePlanner_Service_ServiceId",
                table: "ExpensePlanner",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ExpensePlanner_Devices_DeviceId",
                table: "ExpensePlanner");

            migrationBuilder.DropForeignKey(
                name: "FK_ExpensePlanner_Service_ServiceId",
                table: "ExpensePlanner");

            migrationBuilder.AlterColumn<int>(
                name: "ServiceId",
                table: "ExpensePlanner",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "ExpensePlanner",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "CalibrationCosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DeviceId = table.Column<int>(type: "INTEGER", nullable: false),
                    Amount = table.Column<decimal>(type: "TEXT", nullable: false),
                    CalibrationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Currency = table.Column<string>(type: "TEXT", nullable: false),
                    Year = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CalibrationCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CalibrationCosts_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationCosts_DeviceId",
                table: "CalibrationCosts",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_ExpensePlanner_Devices_DeviceId",
                table: "ExpensePlanner",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ExpensePlanner_Service_ServiceId",
                table: "ExpensePlanner",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
