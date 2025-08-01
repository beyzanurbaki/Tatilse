using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tatilse.Migrations
{
    /// <inheritdoc />
    public partial class client_gneder_added : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<string>(
            //    name: "room_image",
            //    table: "Rooms",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)");

            //migrationBuilder.AlterColumn<string>(
            //    name: "hotel_image",
            //    table: "Hotels",
            //    type: "nvarchar(max)",
            //    nullable: true,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)");

            //migrationBuilder.AlterColumn<string>(
            //    name: "client_name",
            //    table: "Clients",
            //    type: "nvarchar(30)",
            //    maxLength: 30,
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(50)",
            //    oldMaxLength: 50);

            migrationBuilder.AddColumn<bool>(
                name: "client_gender",
                table: "Clients",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "client_gender",
                table: "Clients");

            //migrationBuilder.AlterColumn<string>(
            //    name: "room_image",
            //    table: "Rooms",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "",
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "hotel_image",
            //    table: "Hotels",
            //    type: "nvarchar(max)",
            //    nullable: false,
            //    defaultValue: "",
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(max)",
            //    oldNullable: true);

            //migrationBuilder.AlterColumn<string>(
            //    name: "client_name",
            //    table: "Clients",
            //    type: "nvarchar(50)",
            //    maxLength: 50,
            //    nullable: false,
            //    oldClrType: typeof(string),
            //    oldType: "nvarchar(30)",
            //    oldMaxLength: 30);
        }
    }
}
