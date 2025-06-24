using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASI.Basecode.Data.Migrations
{
    public partial class AddedCreatedByAndUpdatedByOnAdmin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewImage",
                table: "Reviews");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedBy",
                table: "Admins",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Admins");

            migrationBuilder.DropColumn(
                name: "UpdatedBy",
                table: "Admins");

            migrationBuilder.AddColumn<string>(
                name: "ReviewImage",
                table: "Reviews",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
