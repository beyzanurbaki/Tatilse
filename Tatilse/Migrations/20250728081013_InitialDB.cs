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
                    client_username = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    client_name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    client_surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    client_birthdate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    client_identity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    client_phone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    client_email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    client_passw = table.Column<string>(type: "nvarchar(12)", maxLength: 12, nullable: false),
                    isAdmin = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.client_id);
                });

            migrationBuilder.CreateTable(
                name: "Features",
                columns: table => new
                {
                    feature_id = table.Column<byte>(type: "tinyint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    feature_name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Features", x => x.feature_id);
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
                    hotel_city = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hotel_township = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hotel_image = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Hotels", x => x.hotel_id);
                });

            migrationBuilder.CreateTable(
                name: "HotelFeatures",
                columns: table => new
                {
                    Hotelshotel_id = table.Column<int>(type: "int", nullable: false),
                    featuresfeature_id = table.Column<byte>(type: "tinyint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HotelFeatures", x => new { x.Hotelshotel_id, x.featuresfeature_id });
                    table.ForeignKey(
                        name: "FK_HotelFeatures_Features_featuresfeature_id",
                        column: x => x.featuresfeature_id,
                        principalTable: "Features",
                        principalColumn: "feature_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HotelFeatures_Hotels_Hotelshotel_id",
                        column: x => x.Hotelshotel_id,
                        principalTable: "Hotels",
                        principalColumn: "hotel_id",
                        onDelete: ReferentialAction.Cascade);
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
                    room_max_people = table.Column<short>(type: "smallint", nullable: false),
                    room_image = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    hotel_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rooms", x => x.room_id);
                    table.ForeignKey(
                        name: "FK_Rooms_Hotels_hotel_id",
                        column: x => x.hotel_id,
                        principalTable: "Hotels",
                        principalColumn: "hotel_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Reservations",
                columns: table => new
                {
                    reservation_id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    start_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    end_date = table.Column<DateTime>(type: "datetime2", nullable: true),
                    client_id = table.Column<int>(type: "int", nullable: false),
                    room_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reservations", x => x.reservation_id);
                    table.ForeignKey(
                        name: "FK_Reservations_Clients_client_id",
                        column: x => x.client_id,
                        principalTable: "Clients",
                        principalColumn: "client_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Reservations_Rooms_room_id",
                        column: x => x.room_id,
                        principalTable: "Rooms",
                        principalColumn: "room_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HotelFeatures_featuresfeature_id",
                table: "HotelFeatures",
                column: "featuresfeature_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_client_id",
                table: "Reservations",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "IX_Reservations_room_id",
                table: "Reservations",
                column: "room_id");

            migrationBuilder.CreateIndex(
                name: "IX_Rooms_hotel_id",
                table: "Rooms",
                column: "hotel_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HotelFeatures");

            migrationBuilder.DropTable(
                name: "Reservations");

            migrationBuilder.DropTable(
                name: "Features");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Rooms");

            migrationBuilder.DropTable(
                name: "Hotels");
        }
    }
}
