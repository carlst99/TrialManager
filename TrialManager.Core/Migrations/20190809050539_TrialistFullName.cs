using Microsoft.EntityFrameworkCore.Migrations;

namespace TrialManager.Core.Migrations
{
    public partial class TrialistFullName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Trialists");

            migrationBuilder.RenameColumn(
                name: "Surname",
                table: "Trialists",
                newName: "FullName");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Trialists",
                nullable: true,
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Trialists",
                newName: "Surname");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Trialists",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Trialists",
                nullable: false,
                defaultValue: "");
        }
    }
}
