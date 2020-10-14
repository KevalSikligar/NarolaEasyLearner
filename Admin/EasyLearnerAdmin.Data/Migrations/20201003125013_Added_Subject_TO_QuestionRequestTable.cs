using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyLearnerAdmin.Data.Migrations
{
    public partial class Added_Subject_TO_QuestionRequestTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "QuestionRequests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "QuestionRequests");
        }
    }
}
