using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace tictax.api.Data.Migrations
{
    public partial class RemovedTokenColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenCreated",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TokenExpires",
                table: "User");

            migrationBuilder.AddColumn<int>(
                name: "TotalGames",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalLoses",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalWins",
                table: "User",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalGames",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TotalLoses",
                table: "User");

            migrationBuilder.DropColumn(
                name: "TotalWins",
                table: "User");

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenCreated",
                table: "User",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "TokenExpires",
                table: "User",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
