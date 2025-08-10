using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PhoneRegistry.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCityAndContactInfoCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CityId",
                table: "ContactInfos",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ContactInfos_CityId",
                table: "ContactInfos",
                column: "CityId");

            migrationBuilder.AddForeignKey(
                name: "FK_ContactInfos_Cities_CityId",
                table: "ContactInfos",
                column: "CityId",
                principalTable: "Cities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ContactInfos_Cities_CityId",
                table: "ContactInfos");

            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropIndex(
                name: "IX_ContactInfos_CityId",
                table: "ContactInfos");

            migrationBuilder.DropColumn(
                name: "CityId",
                table: "ContactInfos");
        }
    }
}
