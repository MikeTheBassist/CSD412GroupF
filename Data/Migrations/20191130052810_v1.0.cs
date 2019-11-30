using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GroupF.Data.Migrations
{
    public partial class v10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "name",
                table: "Game");

            migrationBuilder.AddColumn<DateTime>(
                name: "updated",
                table: "Game",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "updated",
                table: "Game");

            migrationBuilder.AddColumn<string>(
                name: "name",
                table: "Game",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
