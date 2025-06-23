using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASI.Basecode.Data.Migrations
{
    public partial class Replaced_ADminID_and_UpdatedByADminId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedByAdminId",
                table: "Books",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Books",
                newName: "CreatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Books",
                newName: "UpdatedByAdminId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Books",
                newName: "AdminId");
        }
    }
}
