using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyLearnerAdmin.Data.Migrations
{
    public partial class Added_Invitation_Code_Class : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpirationDays",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "NoOfFreeDays",
                table: "Friends");

            migrationBuilder.DropColumn(
                name: "NoOfFreeQuestions",
                table: "Friends");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExpirationDays",
                table: "Friends",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NoOfFreeDays",
                table: "Friends",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NoOfFreeQuestions",
                table: "Friends",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
