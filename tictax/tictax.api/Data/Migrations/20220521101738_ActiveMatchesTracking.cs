using Microsoft.EntityFrameworkCore.Migrations;

namespace tictax.api.Data.Migrations
{
    public partial class ActiveMatchesTracking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Match",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreationTime = table.Column<int>(type: "int", nullable: false),
                    OwnerUsername = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    OpponentUsername = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Match", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Match_User_OpponentUsername",
                        column: x => x.OpponentUsername,
                        principalTable: "User",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Match_User_OwnerUsername",
                        column: x => x.OwnerUsername,
                        principalTable: "User",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Match_OpponentUsername",
                table: "Match",
                column: "OpponentUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Match_OwnerUsername",
                table: "Match",
                column: "OwnerUsername");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Match");
        }
    }
}
