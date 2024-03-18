using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Migrations
{
    public partial class p10 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_usersId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_userId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Lessons_LessonsId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_UsersId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_AspNetUsers_usersId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_CoursescourseId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Courses_CoursescourseId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_CoursescourseId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CoursescourseId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_usersId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_LessonsId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_userId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Cards_usersId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CoursescourseId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "CoursescourseId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "usersId",
                table: "Enrollments");

            migrationBuilder.DropColumn(
                name: "usersId",
                table: "Cards");

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

            migrationBuilder.AddColumn<string>(
                name: "techerId",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "CardValue",
                table: "Cards",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_usersId",
                table: "Courses",
                column: "usersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_usersId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "techerId",
                table: "Courses");

            migrationBuilder.RenameColumn(
                name: "usersId",
                table: "Courses",
                newName: "UsersId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_usersId",
                table: "Courses",
                newName: "IX_Courses_UsersId");

            migrationBuilder.AddColumn<int>(
                name: "CoursescourseId",
                table: "Lessons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CoursescourseId",
                table: "Enrollments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "usersId",
                table: "Enrollments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UsersId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "CardValue",
                table: "Cards",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "usersId",
                table: "Cards",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_CoursescourseId",
                table: "Lessons",
                column: "CoursescourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CoursescourseId",
                table: "Enrollments",
                column: "CoursescourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_usersId",
                table: "Enrollments",
                column: "usersId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_LessonsId",
                table: "Comments",
                column: "LessonsId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_userId",
                table: "Comments",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_usersId",
                table: "Cards",
                column: "usersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_usersId",
                table: "Cards",
                column: "usersId",
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
                name: "FK_Comments_Lessons_LessonsId",
                table: "Comments",
                column: "LessonsId",
                principalTable: "Lessons",
                principalColumn: "LessonsId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_UsersId",
                table: "Courses",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_AspNetUsers_usersId",
                table: "Enrollments",
                column: "usersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_CoursescourseId",
                table: "Enrollments",
                column: "CoursescourseId",
                principalTable: "Courses",
                principalColumn: "courseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Courses_CoursescourseId",
                table: "Lessons",
                column: "CoursescourseId",
                principalTable: "Courses",
                principalColumn: "courseId");
        }
    }
}
