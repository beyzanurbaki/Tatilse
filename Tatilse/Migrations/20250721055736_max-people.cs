using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tatilse.Migrations
{
    /// <inheritdoc />
    public partial class maxpeople : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<short>(
                name: "room_max_people",
                table: "Rooms",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_hotel_id",
                table: "Rooms",
                column: "hotel_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_room_id",
                table: "Reservations",
                column: "room_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservations_Rooms_room_id",
                table: "Reservations",
                column: "room_id",
                principalTable: "Rooms",
                principalColumn: "room_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Rooms_Hotels_hotel_id",
                table: "Rooms",
                column: "hotel_id",
                principalTable: "Hotels",
                principalColumn: "hotel_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservations_Rooms_room_id",
                table: "Reservations");

            migrationBuilder.DropForeignKey(
                name: "FK_Rooms_Hotels_hotel_id",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Rooms_hotel_id",
                table: "Rooms");

            migrationBuilder.DropIndex(
                name: "IX_Reservations_room_id",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "room_max_people",
                table: "Rooms");
        }
    }
}
