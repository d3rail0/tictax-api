using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tictax.api.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Username = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PasswordHash = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    PasswordSalt = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    TotalWins = table.Column<int>(type: "int", nullable: false),
                    TotalLoses = table.Column<int>(type: "int", nullable: false),
                    TotalGames = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Username);
                });

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

            migrationBuilder.CreateTable(
                name: "ProfileActivity",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ActivityTime = table.Column<long>(type: "bigint", nullable: false),
                    UsernameFrom = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UsernameTo = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileActivity", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileActivity_User_UsernameFrom",
                        column: x => x.UsernameFrom,
                        principalTable: "User",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProfileActivity_User_UsernameTo",
                        column: x => x.UsernameTo,
                        principalTable: "User",
                        principalColumn: "Username",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Match_OpponentUsername",
                table: "Match",
                column: "OpponentUsername");

            migrationBuilder.CreateIndex(
                name: "IX_Match_OwnerUsername",
                table: "Match",
                column: "OwnerUsername");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileActivity_UsernameFrom",
                table: "ProfileActivity",
                column: "UsernameFrom");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileActivity_UsernameTo",
                table: "ProfileActivity",
                column: "UsernameTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Match");

            migrationBuilder.DropTable(
                name: "ProfileActivity");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
