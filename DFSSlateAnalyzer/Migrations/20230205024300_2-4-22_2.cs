using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DFSSlateAnalyzerAPI.Migrations
{
    /// <inheritdoc />
    public partial class _24222 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Contests_ContestID",
                table: "Players");

            migrationBuilder.AlterColumn<long>(
                name: "ContestID",
                table: "Players",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Contests_ContestID",
                table: "Players",
                column: "ContestID",
                principalTable: "Contests",
                principalColumn: "ContestID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Contests_ContestID",
                table: "Players");

            migrationBuilder.AlterColumn<int>(
                name: "ContestID",
                table: "Players",
                type: "int",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Contests_ContestID",
                table: "Players",
                column: "ContestID",
                principalTable: "Contests",
                principalColumn: "ID");
        }
    }
}
