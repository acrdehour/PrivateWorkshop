using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrivateWorkshop.Migrations
{
    /// <inheritdoc />
    public partial class editBookingProp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TimeSlot",
                table: "Bookings",
                newName: "Duration");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Bookings",
                newName: "TimeSlot");
        }
    }
}
