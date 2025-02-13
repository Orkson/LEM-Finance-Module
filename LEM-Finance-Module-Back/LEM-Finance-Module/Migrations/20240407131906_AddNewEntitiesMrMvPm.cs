using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TestLEM.Migrations
{
    /// <inheritdoc />
    public partial class AddNewEntitiesMrMvPm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PhysicalMagnitudes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhysicalMagnitudes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MeasuredValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ModelId = table.Column<int>(type: "int", nullable: false),
                    PhysicalMagnitudeId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasuredValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasuredValues_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MeasuredValues_PhysicalMagnitudes_PhysicalMagnitudeId",
                        column: x => x.PhysicalMagnitudeId,
                        principalTable: "PhysicalMagnitudes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MeasuredRanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Range = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccuracyInPercet = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MeasuredValueId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MeasuredRanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MeasuredRanges_MeasuredValues_MeasuredValueId",
                        column: x => x.MeasuredValueId,
                        principalTable: "MeasuredValues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredRanges_MeasuredValueId",
                table: "MeasuredRanges",
                column: "MeasuredValueId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredValues_ModelId",
                table: "MeasuredValues",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_MeasuredValues_PhysicalMagnitudeId",
                table: "MeasuredValues",
                column: "PhysicalMagnitudeId");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalMagnitudes_Name",
                table: "PhysicalMagnitudes",
                column: "Name",
                unique: true,
                filter: "[Name] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_PhysicalMagnitudes_Unit",
                table: "PhysicalMagnitudes",
                column: "Unit",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MeasuredRanges");

            migrationBuilder.DropTable(
                name: "MeasuredValues");

            migrationBuilder.DropTable(
                name: "PhysicalMagnitudes");
        }
    }
}
