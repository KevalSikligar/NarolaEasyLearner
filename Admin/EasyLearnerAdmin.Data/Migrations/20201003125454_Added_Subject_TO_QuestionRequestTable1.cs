using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyLearnerAdmin.Data.Migrations
{
    public partial class Added_Subject_TO_QuestionRequestTable1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "QuestionRequests");

            migrationBuilder.AddColumn<string>(
                name: "SubjectName",
                table: "QuestionRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubjectName",
                table: "QuestionRequests");

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "QuestionRequests",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
