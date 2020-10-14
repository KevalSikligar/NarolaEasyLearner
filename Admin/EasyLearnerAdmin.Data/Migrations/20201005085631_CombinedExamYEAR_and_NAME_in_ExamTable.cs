using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyLearnerAdmin.Data.Migrations
{
    public partial class CombinedExamYEAR_and_NAME_in_ExamTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamName",
                table: "Exams");

            migrationBuilder.DropColumn(
                name: "ExamYear",
                table: "Exams");

            migrationBuilder.AddColumn<string>(
                name: "ExamNameAndYear",
                table: "Exams",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamNameAndYear",
                table: "Exams");

            migrationBuilder.AddColumn<string>(
                name: "ExamName",
                table: "Exams",
                type: "nvarchar(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExamYear",
                table: "Exams",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");
        }
    }
}
