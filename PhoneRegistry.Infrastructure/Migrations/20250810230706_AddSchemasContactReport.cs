using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhoneRegistry.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSchemasContactReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "contact");

            migrationBuilder.EnsureSchema(
                name: "report");

            migrationBuilder.RenameTable(
                name: "Reports",
                newName: "Reports",
                newSchema: "report");

            migrationBuilder.RenameTable(
                name: "Persons",
                newName: "Persons",
                newSchema: "contact");

            migrationBuilder.RenameTable(
                name: "LocationStatistics",
                newName: "LocationStatistics",
                newSchema: "report");

            migrationBuilder.RenameTable(
                name: "ContactInfos",
                newName: "ContactInfos",
                newSchema: "contact");

            migrationBuilder.RenameTable(
                name: "Cities",
                newName: "Cities",
                newSchema: "contact");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Reports",
                schema: "report",
                newName: "Reports");

            migrationBuilder.RenameTable(
                name: "Persons",
                schema: "contact",
                newName: "Persons");

            migrationBuilder.RenameTable(
                name: "LocationStatistics",
                schema: "report",
                newName: "LocationStatistics");

            migrationBuilder.RenameTable(
                name: "ContactInfos",
                schema: "contact",
                newName: "ContactInfos");

            migrationBuilder.RenameTable(
                name: "Cities",
                schema: "contact",
                newName: "Cities");
        }
    }
}
