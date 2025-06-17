using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASI.Basecode.Data.Migrations
{
    public partial class Addding_New_Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UpdatedByAdminId",
                table: "Books",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UpdatedByAdminId",
                table: "BookGenres",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedByAdminId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "UpdatedByAdminId",
                table: "BookGenres");
        }
    }
}
