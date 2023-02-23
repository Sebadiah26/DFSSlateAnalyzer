using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DFSSlateAnalyzerAPI.Migrations
{
    /// <inheritdoc />
    public partial class _211226666666667774643664335433 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dfs");

            migrationBuilder.CreateTable(
                name: "Contest",
                schema: "dfs",
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
                    table.PrimaryKey("PK_Contest", x => x.ID);
                    table.UniqueConstraint("AK_Contest_ContestID", x => x.ContestID);
                });

            migrationBuilder.CreateTable(
                name: "DFSPlayer",
                schema: "dfs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerID = table.Column<int>(type: "int", nullable: false),
                    BMFirstName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BMLastName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DKPlayerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Team = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RosterPosition = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DFSPlayer", x => x.ID);
                    table.UniqueConstraint("AK_DFSPlayer_PlayerID", x => x.PlayerID);
                });

            migrationBuilder.CreateTable(
                name: "Owner",
                schema: "dfs",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Owner", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Entry",
                schema: "dfs",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntryID = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TimeRemaining = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Lineup = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Points = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ContestID = table.Column<long>(type: "bigint", nullable: false),
                    Rank = table.Column<int>(type: "int", nullable: false),
                    OwnerName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Entry", x => x.ID);
                    table.UniqueConstraint("AK_Entry_EntryID", x => x.EntryID);
                    table.ForeignKey(
                        name: "FK_Entry_Contest_ContestID",
                        column: x => x.ContestID,
                        principalSchema: "dfs",
                        principalTable: "Contest",
                        principalColumn: "ContestID");
                    table.ForeignKey(
                        name: "FK_Entry_Owner_OwnerName",
                        column: x => x.OwnerName,
                        principalSchema: "dfs",
                        principalTable: "Owner",
                        principalColumn: "Name");
                });

            migrationBuilder.CreateTable(
                name: "EntryMember",
                schema: "dfs",
                columns: table => new
                {
                    EntryID = table.Column<long>(type: "bigint", nullable: false),
                    EntryMemberPlayerName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContestID = table.Column<long>(type: "bigint", nullable: false),
                    LineupSlot = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntryMember", x => new { x.ContestID, x.EntryID, x.EntryMemberPlayerName });
                    table.ForeignKey(
                        name: "FK_EntryMember_Entry_EntryID",
                        column: x => x.EntryID,
                        principalSchema: "dfs",
                        principalTable: "Entry",
                        principalColumn: "EntryID");
                });

            migrationBuilder.CreateTable(
                name: "Player",
                schema: "dfs",
                columns: table => new
                {
                    PlayerName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ContestID = table.Column<long>(type: "bigint", nullable: false),
                    Salary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Points = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProjectedPoints = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Position = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RosterPosition = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Drafted = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FPTS = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PlayerID = table.Column<int>(type: "int", nullable: true),
                    BMProjection = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OPP = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    entryMemberContestID = table.Column<long>(type: "bigint", nullable: true),
                    entryMemberEntryID = table.Column<long>(type: "bigint", nullable: true),
                    EntryMemberPlayerName = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Player", x => new { x.PlayerName, x.ContestID });
                    table.ForeignKey(
                        name: "FK_Player_Contest_ContestID",
                        column: x => x.ContestID,
                        principalSchema: "dfs",
                        principalTable: "Contest",
                        principalColumn: "ContestID");
                    table.ForeignKey(
                        name: "FK_Player_DFSPlayer_PlayerID",
                        column: x => x.PlayerID,
                        principalSchema: "dfs",
                        principalTable: "DFSPlayer",
                        principalColumn: "PlayerID");
                    table.ForeignKey(
                        name: "FK_Player_EntryMember_entryMemberContestID_entryMemberEntryID_EntryMemberPlayerName",
                        columns: x => new { x.entryMemberContestID, x.entryMemberEntryID, x.EntryMemberPlayerName },
                        principalSchema: "dfs",
                        principalTable: "EntryMember",
                        principalColumns: new[] { "ContestID", "EntryID", "EntryMemberPlayerName" });
                });

            migrationBuilder.CreateIndex(
                name: "IX_Entry_ContestID",
                schema: "dfs",
                table: "Entry",
                column: "ContestID");

            migrationBuilder.CreateIndex(
                name: "IX_Entry_OwnerName",
                schema: "dfs",
                table: "Entry",
                column: "OwnerName");

            migrationBuilder.CreateIndex(
                name: "IX_EntryMember_EntryID",
                schema: "dfs",
                table: "EntryMember",
                column: "EntryID");

            migrationBuilder.CreateIndex(
                name: "IX_Player_ContestID",
                schema: "dfs",
                table: "Player",
                column: "ContestID");

            migrationBuilder.CreateIndex(
                name: "IX_Player_entryMemberContestID_entryMemberEntryID_EntryMemberPlayerName",
                schema: "dfs",
                table: "Player",
                columns: new[] { "entryMemberContestID", "entryMemberEntryID", "EntryMemberPlayerName" });

            migrationBuilder.CreateIndex(
                name: "IX_Player_PlayerID",
                schema: "dfs",
                table: "Player",
                column: "PlayerID",
                unique: true,
                filter: "[PlayerID] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Player",
                schema: "dfs");

            migrationBuilder.DropTable(
                name: "DFSPlayer",
                schema: "dfs");

            migrationBuilder.DropTable(
                name: "EntryMember",
                schema: "dfs");

            migrationBuilder.DropTable(
                name: "Entry",
                schema: "dfs");

            migrationBuilder.DropTable(
                name: "Contest",
                schema: "dfs");

            migrationBuilder.DropTable(
                name: "Owner",
                schema: "dfs");
        }
    }
}
