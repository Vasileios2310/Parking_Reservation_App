using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ParkingReservationApp.Migrations
{
    /// <inheritdoc />
    public partial class AddOverdueFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOverdue",
                table: "Reservations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOverdueCharged",
                table: "Reservations",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOverdue",
                table: "Reservations");

            migrationBuilder.DropColumn(
                name: "IsOverdueCharged",
                table: "Reservations");
        }
    }
}
