using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Migrations
{
    public partial class p2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_Id",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_Id",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_UsersId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_AspNetUsers_Id",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_Id",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Enrollments",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_Id",
                table: "Enrollments",
                newName: "IX_Enrollments_userId");

            migrationBuilder.RenameColumn(
                name: "UsersId",
                table: "Courses",
                newName: "usersId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_UsersId",
                table: "Courses",
                newName: "IX_Courses_usersId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Cards",
                newName: "userId");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_Id",
                table: "Cards",
                newName: "IX_Cards_userId");

            migrationBuilder.AlterColumn<string>(
                name: "usersId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<string>(
                name: "userId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_userId",
                table: "Comments",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_userId",
                table: "Cards",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_userId",
                table: "Comments",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_usersId",
                table: "Courses",
                column: "usersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_AspNetUsers_userId",
                table: "Enrollments",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_userId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_userId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_usersId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_AspNetUsers_userId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_userId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "userId",
                table: "Comments");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Enrollments",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_userId",
                table: "Enrollments",
                newName: "IX_Enrollments_Id");

            migrationBuilder.RenameColumn(
                name: "usersId",
                table: "Courses",
                newName: "UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_usersId",
                table: "Courses",
                newName: "IX_Courses_UsersId");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Cards",
                newName: "Id");

            migrationBuilder.RenameIndex(
                name: "IX_Cards_userId",
                table: "Cards",
                newName: "IX_Cards_Id");

            migrationBuilder.AlterColumn<string>(
                name: "UsersId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comments_Id",
                table: "Comments",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_Id",
                table: "Cards",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_Id",
                table: "Comments",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_UsersId",
                table: "Courses",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_AspNetUsers_Id",
                table: "Enrollments",
                column: "Id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
