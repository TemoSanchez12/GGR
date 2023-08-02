using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GGR.Server.Data.Migrations
{
    /// <inheritdoc />
    public partial class RegistrationCorrections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Registrations",
                newName: "VerificationToken");

            migrationBuilder.AddColumn<string>(
                name: "VerificationPhoneCode",
                table: "Registrations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VerificationPhoneCode",
                table: "Registrations");

            migrationBuilder.RenameColumn(
                name: "VerificationToken",
                table: "Registrations",
                newName: "Code");
        }
    }
}
