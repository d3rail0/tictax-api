using Microsoft.EntityFrameworkCore.Migrations;

namespace tictax.api.Data.Migrations
{
    public partial class FixCascade : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileActivity_User_UsernameFrom",
                table: "ProfileActivity");

            migrationBuilder.AlterColumn<string>(
                name: "UsernameFrom",
                table: "ProfileActivity",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileActivity_User_UsernameFrom",
                table: "ProfileActivity",
                column: "UsernameFrom",
                principalTable: "User",
                principalColumn: "Username",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProfileActivity_User_UsernameFrom",
                table: "ProfileActivity");

            migrationBuilder.AlterColumn<string>(
                name: "UsernameFrom",
                table: "ProfileActivity",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProfileActivity_User_UsernameFrom",
                table: "ProfileActivity",
                column: "UsernameFrom",
                principalTable: "User",
                principalColumn: "Username",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
