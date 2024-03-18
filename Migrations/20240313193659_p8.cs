using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Migrations
{
    public partial class p8 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_usersId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "usersId",
                table: "Courses",
                newName: "UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_usersId",
                table: "Courses",
                newName: "IX_Courses_UsersId");

            migrationBuilder.AlterColumn<string>(
                name: "UsersId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_UsersId",
                table: "Courses",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_UsersId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "Courses",
                newName: "usersId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_UsersId",
                table: "Courses",
                newName: "IX_Courses_usersId");

            migrationBuilder.AlterColumn<string>(
                name: "usersId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_usersId",
                table: "Courses",
                column: "usersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
