using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASI.Basecode.Data.Migrations
{
    public partial class ChangedUpdatedByandAdminId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedByAdminId",
                table: "Languages",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Languages",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedByAdminId",
                table: "BookGenres",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "BookGenres",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedByAdminId",
                table: "Authors",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "Authors",
                newName: "CreatedBy");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Languages",
                newName: "UpdatedByAdminId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Languages",
                newName: "AdminId");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "BookGenres",
                newName: "UpdatedByAdminId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "BookGenres",
                newName: "AdminId");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Authors",
                newName: "UpdatedByAdminId");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Authors",
                newName: "AdminId");
        }
    }
}
