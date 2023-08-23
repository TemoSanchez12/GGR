using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGR.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddIsProcessedFileRecord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SaleTickets",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Folio",
                table: "SaleTickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "SaleTickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "FileRecords",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SaleTickets");

            migrationBuilder.DropColumn(
                name: "Folio",
                table: "SaleTickets");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "SaleTickets");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "FileRecords");
        }
    }
}
