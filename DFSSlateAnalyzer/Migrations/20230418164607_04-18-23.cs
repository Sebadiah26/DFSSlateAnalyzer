using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DFSSlateAnalyzerAPI.Migrations
{
    /// <inheritdoc />
    public partial class _041823 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Player_PlayerID",
                schema: "dfs",
                table: "Player");

            migrationBuilder.CreateIndex(
                name: "IX_Player_PlayerID",
                schema: "dfs",
                table: "Player",
                column: "PlayerID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Player_PlayerID",
                schema: "dfs",
                table: "Player");

            migrationBuilder.CreateIndex(
                name: "IX_Player_PlayerID",
                schema: "dfs",
                table: "Player",
                column: "PlayerID",
                unique: true,
                filter: "[PlayerID] IS NOT NULL");
        }
    }
}
