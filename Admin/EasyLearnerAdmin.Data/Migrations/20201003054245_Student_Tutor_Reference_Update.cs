using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyLearnerAdmin.Data.Migrations
{
    public partial class Student_Tutor_Reference_Update : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionRequests_Students_StudentId",
                table: "QuestionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionResponses_Tutors_TutorId",
                table: "QuestionResponses");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionRequests_AspNetUsers_StudentId",
                table: "QuestionRequests",
                column: "StudentId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionResponses_AspNetUsers_TutorId",
                table: "QuestionResponses",
                column: "TutorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_QuestionRequests_AspNetUsers_StudentId",
                table: "QuestionRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_QuestionResponses_AspNetUsers_TutorId",
                table: "QuestionResponses");

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionRequests_Students_StudentId",
                table: "QuestionRequests",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_QuestionResponses_Tutors_TutorId",
                table: "QuestionResponses",
                column: "TutorId",
                principalTable: "Tutors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
