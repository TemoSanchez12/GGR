using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGR.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeToTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HourAndMinutesRegister",
                table: "SaleTickets",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HourAndMinutesRegister",
                table: "SaleTickets");
        }
    }
}
