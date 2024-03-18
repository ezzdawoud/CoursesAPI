using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Courses.Migrations
{
    public partial class p9 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_userId",
                table: "Cards");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_AspNetUsers_userId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Enrollments_Courses_CouresId",
                table: "Enrollments");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Courses_courseId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_courseId",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_CouresId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Enrollments_userId",
                table: "Enrollments");

            migrationBuilder.DropIndex(
                name: "IX_Cards_userId",
                table: "Cards");

            migrationBuilder.RenameColumn(
                name: "userId",
                table: "Enrollments",
                newName: "UserId");

            migrationBuilder.AddColumn<int>(
                name: "CoursescourseId",
                table: "Lessons",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

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
                name: "userId",
                table: "Cards",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_AspNetUsers_usersId",
                table: "Cards");

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
                name: "UserId",
                table: "Enrollments",
                newName: "userId");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "Enrollments",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "userId",
                table: "Cards",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_courseId",
                table: "Lessons",
                column: "courseId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_CouresId",
                table: "Enrollments",
                column: "CouresId");

            migrationBuilder.CreateIndex(
                name: "IX_Enrollments_userId",
                table: "Enrollments",
                column: "userId");

            migrationBuilder.CreateIndex(
                name: "IX_Cards_userId",
                table: "Cards",
                column: "userId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_AspNetUsers_userId",
                table: "Cards",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_AspNetUsers_userId",
                table: "Enrollments",
                column: "userId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Enrollments_Courses_CouresId",
                table: "Enrollments",
                column: "CouresId",
                principalTable: "Courses",
                principalColumn: "courseId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Courses_courseId",
                table: "Lessons",
                column: "courseId",
                principalTable: "Courses",
                principalColumn: "courseId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
