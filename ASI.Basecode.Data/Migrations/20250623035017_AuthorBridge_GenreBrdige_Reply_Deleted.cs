using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASI.Basecode.Data.Migrations
{
    public partial class AuthorBridge_GenreBrdige_Reply_Deleted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookAuthorBridges");

            migrationBuilder.DropTable(
                name: "BookGenreBridges");

            migrationBuilder.DropTable(
                name: "Replies");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookAuthorBridges",
                columns: table => new
                {
                    BookAuthorBridgeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AuthorId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookAuthorBridges", x => x.BookAuthorBridgeId);
                });

            migrationBuilder.CreateTable(
                name: "BookGenreBridges",
                columns: table => new
                {
                    BookGenreBridgeId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BookGenreId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BookId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookGenreBridges", x => x.BookGenreBridgeId);
                });

            migrationBuilder.CreateTable(
                name: "Replies",
                columns: table => new
                {
                    ReplyId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    AdminId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Likes = table.Column<int>(type: "int", nullable: false),
                    ReplyContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReviewImage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UploadDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Replies", x => x.ReplyId);
                });
        }
    }
}
