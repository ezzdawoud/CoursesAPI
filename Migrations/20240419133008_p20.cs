using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Migrations
{
    public partial class p20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "Cards",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cards_UsersId",
                table: "Cards",
                column: "UsersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_UsersId",
                table: "Cards",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_UsersId",
                table: "Cards");

            migrationBuilder.DropIndex(
                name: "IX_Cards_UsersId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Cards");
        }
    }
}
