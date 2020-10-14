using Microsoft.EntityFrameworkCore.Migrations;

namespace EasyLearnerAdmin.Data.Migrations
{
    public partial class RemovedMemberShipStatusfromStudent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MemberShipStatus",
                table: "Students");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "MemberShipStatus",
                table: "Students",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
