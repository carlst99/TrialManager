using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrialManager.Core.Migrations
{
    public partial class AddProperties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "PreferredDay",
                table: "Trialists",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "TravellingPartnerTrialistId",
                table: "Trialists",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trialists_TravellingPartnerTrialistId",
                table: "Trialists",
                column: "TravellingPartnerTrialistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trialists_Trialists_TravellingPartnerTrialistId",
                table: "Trialists",
                column: "TravellingPartnerTrialistId",
                principalTable: "Trialists",
                principalColumn: "TrialistId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trialists_Trialists_TravellingPartnerTrialistId",
                table: "Trialists");

            migrationBuilder.DropIndex(
                name: "IX_Trialists_TravellingPartnerTrialistId",
                table: "Trialists");

            migrationBuilder.DropColumn(
                name: "PreferredDay",
                table: "Trialists");

            migrationBuilder.DropColumn(
                name: "TravellingPartnerTrialistId",
                table: "Trialists");
        }
    }
}
