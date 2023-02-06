using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DFSSlateAnalyzerAPI.Migrations
{
    /// <inheritdoc />
    public partial class _222 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_Contests_EntryID",
                table: "Entries");

            migrationBuilder.AddColumn<long>(
                name: "ContestID",
                table: "Entries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Entries_ContestID",
                table: "Entries",
                column: "ContestID");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_Contests_ContestID",
                table: "Entries",
                column: "ContestID",
                principalTable: "Contests",
                principalColumn: "ContestID",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Entries_Contests_ContestID",
                table: "Entries");

            migrationBuilder.DropIndex(
                name: "IX_Entries_ContestID",
                table: "Entries");

            migrationBuilder.DropColumn(
                name: "ContestID",
                table: "Entries");

            migrationBuilder.AddForeignKey(
                name: "FK_Entries_Contests_EntryID",
                table: "Entries",
                column: "EntryID",
                principalTable: "Contests",
                principalColumn: "ContestID",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
