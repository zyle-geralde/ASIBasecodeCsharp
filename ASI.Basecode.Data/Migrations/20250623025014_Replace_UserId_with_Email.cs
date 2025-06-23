using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASI.Basecode.Data.Migrations
{
    public partial class Replace_UserId_with_Email : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Users",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Admins",
                newName: "Email");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedTime",
                table: "Admins",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedTime",
                table: "Admins",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedTime",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "UpdatedTime",
                table: "Admins");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Users",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Admins",
                newName: "AdminId");
        }
    }
}
