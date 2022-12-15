using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AboutNow.Data.Migrations
{
    public partial class NM : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CommentId",
                table: "Comments",
                newName: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Comments",
                newName: "CommentId");
        }
    }
}
