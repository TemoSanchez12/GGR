using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGR.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddLitersAndProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Liters",
                table: "SaleRecords",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "Product",
                table: "SaleRecords",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartDate",
                table: "SaleRecords",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Liters",
                table: "SaleRecords");

            migrationBuilder.DropColumn(
                name: "Product",
                table: "SaleRecords");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "SaleRecords");
        }
    }
}
