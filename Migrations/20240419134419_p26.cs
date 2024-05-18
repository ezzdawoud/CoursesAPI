using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Migrations
{
    public partial class p26 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<int>(
                name: "CoursescourseId",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Enrollments",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UsersId",
                table: "Courses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "UsersId",
                table: "Comments",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "Cards",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_CoursescourseId",
                table: "Lessons",
                column: "CoursescourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CouresId",
                table: "Enrollments",
                column: "CouresId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_UserId",
                table: "Enrollments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_UsersId",
                table: "Courses",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_LessonsId",
                table: "Comments",
                column: "LessonsId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_UsersId",
                table: "Comments",
                column: "UsersId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_userId",
                table: "Cards",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_userId",
                table: "Cards",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Comments_AspNetUsers_UsersId",
                table: "Comments",
                column: "UsersId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

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
                name: "FK_Enrollments_AspNetUsers_UserId",
                table: "Enrollments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_CouresId",
                table: "Enrollments",
                column: "CouresId",
                principalTable: "Courses",
                principalColumn: "courseId",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Courses_CoursescourseId",
                table: "Lessons",
                column: "CoursescourseId",
                principalTable: "Courses",
                principalColumn: "courseId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_userId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_AspNetUsers_UsersId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Comments_Lessons_LessonsId",
                table: "Comments");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_UsersId",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_AspNetUsers_UserId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_CouresId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Courses_CoursescourseId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_CoursescourseId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CouresId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_UserId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Courses_UsersId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Comments_LessonsId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Comments_UsersId",
                table: "Comments");

            migrationBuilder.DropIndex(
                name: "IX_Cards_userId",
                table: "Cards");

            migrationBuilder.DropColumn(
                name: "CoursescourseId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "UsersId",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "UsersId",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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
    }
}
