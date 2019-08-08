using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TrialManager.Core.Migrations
{
    public partial class AddlocationandaddressfieldstoTrialist : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Trialists",
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "Location",
                table: "Trialists",
                nullable: false,
                defaultValue: new byte[] {  });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "Trialists");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Trialists");
        }
    }
}
