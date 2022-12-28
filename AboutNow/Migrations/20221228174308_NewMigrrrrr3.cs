using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AboutNow.Migrations
{
    public partial class NewMigrrrrr3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "User2_Id",
                table: "Friends",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "User1_Id",
                table: "Friends",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Friends_User1_Id",
                table: "Friends",
                column: "User1_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Friends_User2_Id",
                table: "Friends",
                column: "User2_Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_AspNetUsers_User1_Id",
                table: "Friends",
                column: "User1_Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friends_AspNetUsers_User2_Id",
                table: "Friends",
                column: "User2_Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Friends_AspNetUsers_User1_Id",
                table: "Friends");

            migrationBuilder.DropForeignKey(
                name: "FK_Friends_AspNetUsers_User2_Id",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Friends_User1_Id",
                table: "Friends");

            migrationBuilder.DropIndex(
                name: "IX_Friends_User2_Id",
                table: "Friends");

            migrationBuilder.AlterColumn<string>(
                name: "User2_Id",
                table: "Friends",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "User1_Id",
                table: "Friends",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }
    }
}
