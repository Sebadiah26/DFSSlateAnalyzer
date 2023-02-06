using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DFSSlateAnalyzerAPI.Migrations
{
    /// <inheritdoc />
    public partial class _22 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Contests",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ContestID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PercentComplete = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Size = table.Column<int>(type: "int", nullable: true),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Contests", x => x.ID);
                    table.UniqueConstraint("AK_Contests_ContestID", x => x.ContestID);
                });

            migrationBuilder.CreateTable(
                name: "Owners",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owners", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    PlayerName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Salary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Points = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProjectedPoints = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RosterPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Drafted = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FPTS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContestID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.PlayerName);
                    table.ForeignKey(
                        name: "FK_Players_Contests_ContestID",
                        column: x => x.ContestID,
                        principalTable: "Contests",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateTable(
                name: "Entries",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeRemaining = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Lineup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Points = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entries", x => x.ID);
                    table.UniqueConstraint("AK_Entries_EntryID", x => x.EntryID);
                    table.ForeignKey(
                        name: "FK_Entries_Contests_EntryID",
                        column: x => x.EntryID,
                        principalTable: "Contests",
                        principalColumn: "ContestID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Entries_Owners_OwnerName",
                        column: x => x.OwnerName,
                        principalTable: "Owners",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "EntryMembers",
                columns: table => new
                {
                    EntryID = table.Column<long>(type: "bigint", nullable: false),
                    EntryMemberPlayerName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LineupSlot = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryMembers", x => new { x.EntryID, x.EntryMemberPlayerName });
                    table.ForeignKey(
                        name: "FK_EntryMembers_Entries_EntryID",
                        column: x => x.EntryID,
                        principalTable: "Entries",
                        principalColumn: "EntryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entries_OwnerName",
                table: "Entries",
                column: "OwnerName");

            migrationBuilder.CreateIndex(
                name: "IX_Players_ContestID",
                table: "Players",
                column: "ContestID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EntryMembers");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Entries");

            migrationBuilder.DropTable(
                name: "Contests");

            migrationBuilder.DropTable(
                name: "Owners");
        }
    }
}
