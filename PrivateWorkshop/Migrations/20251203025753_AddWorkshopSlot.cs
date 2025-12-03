using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrivateWorkshop.Migrations
{
    /// <inheritdoc />
    public partial class AddWorkshopSlot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkshopSlots",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkshopId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Duration = table.Column<int>(type: "int", nullable: false),
                    BookedCount = table.Column<int>(type: "int", nullable: false),
                    MaxSlot = table.Column<int>(type: "int", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkshopSlots", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkshopSlots_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkshopSlots_WorkshopId_Date_Duration",
                table: "WorkshopSlots",
                columns: new[] { "WorkshopId", "Date", "Duration" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkshopSlots");
        }
    }
}
