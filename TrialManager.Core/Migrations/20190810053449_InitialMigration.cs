using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrialManager.Core.Migrations
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
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FullName = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    Address = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Dogs = table.Column<byte[]>(nullable: false),
                    Location = table.Column<byte[]>(nullable: false),
                    PreferredDay = table.Column<DateTime>(nullable: false),
                    TravellingPartnerTrialistId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trialists", x => x.TrialistId);
                    table.ForeignKey(
                        name: "FK_Trialists_Trialists_TravellingPartnerTrialistId",
                        column: x => x.TravellingPartnerTrialistId,
                        principalTable: "Trialists",
                        principalColumn: "TrialistId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trialists_TravellingPartnerTrialistId",
                table: "Trialists",
                column: "TravellingPartnerTrialistId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trialists");
        }
    }
}
