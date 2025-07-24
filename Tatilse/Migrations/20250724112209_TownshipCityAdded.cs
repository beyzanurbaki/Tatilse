using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tatilse.Migrations
{
    /// <inheritdoc />
    public partial class TownshipCityAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "hotel_city",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "hotel_township",
                table: "Hotels",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "hotel_city",
                table: "Hotels");

            migrationBuilder.DropColumn(
                name: "hotel_township",
                table: "Hotels");
        }
    }
}
