using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeGuidToInt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalibrationCosts_Devices_DeviceId1",
                table: "CalibrationCosts");

            migrationBuilder.DropIndex(
                name: "IX_CalibrationCosts_DeviceId1",
                table: "CalibrationCosts");

            migrationBuilder.DropColumn(
                name: "DeviceId1",
                table: "CalibrationCosts");

            migrationBuilder.AlterColumn<int>(
                name: "DeviceId",
                table: "CalibrationCosts",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "CalibrationCosts",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "TEXT")
                .Annotation("Sqlite:Autoincrement", true);

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationCosts_DeviceId",
                table: "CalibrationCosts",
                column: "DeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_CalibrationCosts_Devices_DeviceId",
                table: "CalibrationCosts",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CalibrationCosts_Devices_DeviceId",
                table: "CalibrationCosts");

            migrationBuilder.DropIndex(
                name: "IX_CalibrationCosts_DeviceId",
                table: "CalibrationCosts");

            migrationBuilder.AlterColumn<Guid>(
                name: "DeviceId",
                table: "CalibrationCosts",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "CalibrationCosts",
                type: "TEXT",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "INTEGER")
                .OldAnnotation("Sqlite:Autoincrement", true);

            migrationBuilder.AddColumn<int>(
                name: "DeviceId1",
                table: "CalibrationCosts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CalibrationCosts_DeviceId1",
                table: "CalibrationCosts",
                column: "DeviceId1");

            migrationBuilder.AddForeignKey(
                name: "FK_CalibrationCosts_Devices_DeviceId1",
                table: "CalibrationCosts",
                column: "DeviceId1",
                principalTable: "Devices",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
