using Microsoft.EntityFrameworkCore.Migrations;

namespace UIConcepts.Core.Migrations
{
    public partial class InitialMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trialists",
                columns: table => new
                {
                    TrialistId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(nullable: true),
                    Surname = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trialists", x => x.TrialistId);
                });

            migrationBuilder.CreateTable(
                name: "Dog",
                columns: table => new
                {
                    DogId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    TrialistId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Dog", x => x.DogId);
                    table.ForeignKey(
                        name: "FK_Dog_Trialists_TrialistId",
                        column: x => x.TrialistId,
                        principalTable: "Trialists",
                        principalColumn: "TrialistId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Dog_TrialistId",
                table: "Dog",
                column: "TrialistId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Dog");

            migrationBuilder.DropTable(
                name: "Trialists");
        }
    }
}
