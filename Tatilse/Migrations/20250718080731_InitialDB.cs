using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tatilse.Migrations
{
    /// <inheritdoc />
    public partial class InitialDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    client_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    client_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    client_surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    client_birthdate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    client_identity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    client_phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    client_email = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.client_id);
                });

            migrationBuilder.CreateTable(
                name: "Hotels",
                columns: table => new
                {
                    hotel_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    hotel_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hotel_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    hotel_description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hotel_image = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.hotel_id);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    reservation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    start_date = table.Column<DateOnly>(type: "date", nullable: false),
                    end_date = table.Column<DateOnly>(type: "date", nullable: false),
                    client_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.reservation_id);
                });

            migrationBuilder.CreateTable(
                name: "Rooms",
                columns: table => new
                {
                    room_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    room_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    room_price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    room_quantity = table.Column<short>(type: "smallint", nullable: false),
                    room_capacity = table.Column<short>(type: "smallint", nullable: false),
                    room_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hotel_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.room_id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    feature_id = table.Column<byte>(type: "tinyint", nullable: false),
                    feature_name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    feature_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hotel_id = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.feature_id);
                    table.ForeignKey(
                        name: "FK_Features_Hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "Hotels",
                        principalColumn: "hotel_id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Features_hotel_id",
                table: "Features",
                column: "hotel_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Hotels");
        }
    }
}
