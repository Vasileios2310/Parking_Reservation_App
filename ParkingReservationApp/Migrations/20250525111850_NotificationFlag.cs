using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingReservationApp.Migrations
{
    /// <inheritdoc />
    public partial class NotificationFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsEndNotified",
                table: "Reservations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsStartNotified",
                table: "Reservations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsEndNotified",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsStartNotified",
                table: "Reservations");
        }
    }
}
