using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Migrations
{
    public partial class p12 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_usersId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_usersId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "techerId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "usersId",
                table: "Courses",
                newName: "teacherId");

            migrationBuilder.AlterColumn<string>(
                name: "teacherId",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "Courses",
                newName: "teacherId");

            migrationBuilder.AlterColumn<string>(
                name: "teacherId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "techerId",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_usersId",
                table: "Courses",
                column: "usersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_usersId",
                table: "Courses",
                column: "teacherId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
