using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DFSSlateAnalyzerAPI.Migrations
{
    /// <inheritdoc />
    public partial class _24225 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BMProjection",
                table: "Players",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OPP",
                table: "Players",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PlayerID",
                table: "Players",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BMProjection",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "OPP",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "PlayerID",
                table: "Players");
        }
    }
}
