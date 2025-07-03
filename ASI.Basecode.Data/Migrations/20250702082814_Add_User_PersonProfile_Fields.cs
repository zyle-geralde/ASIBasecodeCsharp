using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASI.Basecode.Data.Migrations
{
    public partial class Add_User_PersonProfile_Fields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
               name: "PK_PersonProfiles",
               table: "PersonProfiles");
            migrationBuilder.AlterColumn<string>(
                name: "ProfileID",
                table: "PersonProfiles",
                type: "varchar(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
            migrationBuilder.AddPrimaryKey(
                 name: "PK_PersonProfiles",
                 table: "PersonProfiles",
                 column: "ProfileID");
            migrationBuilder.AddUniqueConstraint(
                name: "AK_Users_Email",
                table: "Users",
                column: "Email");

            migrationBuilder.AddForeignKey(
                name: "FK_PersonProfiles_Users_ProfileID",
                table: "PersonProfiles",
                column: "ProfileID",
                principalTable: "Users",
                principalColumn: "Email",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PersonProfiles_Users_ProfileID",
                table: "PersonProfiles");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_Users_Email",
                table: "Users");
            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonProfiles",
                table: "PersonProfiles");
            migrationBuilder.AlterColumn<string>(
                name: "ProfileID",
                table: "PersonProfiles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(50)");
            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonProfiles",
                table: "PersonProfiles",
                column: "ProfileID");
        }
    }
}
