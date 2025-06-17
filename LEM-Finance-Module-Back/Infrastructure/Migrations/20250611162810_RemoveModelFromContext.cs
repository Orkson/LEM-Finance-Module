using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveModelFromContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Models_ModelId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Models_ModelId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasuredValues_Models_ModelId",
                table: "MeasuredValues");

            migrationBuilder.DropIndex(
                name: "IX_Models_Name",
                table: "Models");

            migrationBuilder.DropIndex(
                name: "IX_Models_SerialNumber",
                table: "Models");

            migrationBuilder.AddColumn<int>(
                name: "DeviceId",
                table: "MeasuredValues",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "ModelId",
                table: "Devices",
                type: "INTEGER",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<int>(
                name: "CompanyId",
                table: "Devices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 7);

            migrationBuilder.AddColumn<string>(
                name: "SerialNumber",
                table: "Devices",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredValues_DeviceId",
                table: "MeasuredValues",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_Devices_CompanyId",
                table: "Devices",
                column: "CompanyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Companies_CompanyId",
                table: "Devices",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Models_ModelId",
                table: "Devices",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Models_ModelId",
                table: "Documents",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuredValues_Devices_DeviceId",
                table: "MeasuredValues",
                column: "DeviceId",
                principalTable: "Devices",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuredValues_Models_ModelId",
                table: "MeasuredValues",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Companies_CompanyId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Devices_Models_ModelId",
                table: "Devices");

            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Models_ModelId",
                table: "Documents");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasuredValues_Devices_DeviceId",
                table: "MeasuredValues");

            migrationBuilder.DropForeignKey(
                name: "FK_MeasuredValues_Models_ModelId",
                table: "MeasuredValues");

            migrationBuilder.DropIndex(
                name: "IX_MeasuredValues_DeviceId",
                table: "MeasuredValues");

            migrationBuilder.DropIndex(
                name: "IX_Devices_CompanyId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "DeviceId",
                table: "MeasuredValues");

            migrationBuilder.DropColumn(
                name: "CompanyId",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "SerialNumber",
                table: "Devices");

            migrationBuilder.AlterColumn<int>(
                name: "ModelId",
                table: "Devices",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "INTEGER",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Models_Name",
                table: "Models",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Models_SerialNumber",
                table: "Models",
                column: "SerialNumber",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_Models_ModelId",
                table: "Devices",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Models_ModelId",
                table: "Documents",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_MeasuredValues_Models_ModelId",
                table: "MeasuredValues",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
