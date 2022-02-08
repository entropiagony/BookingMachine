using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domain.Migrations
{
    public partial class AddedFloorIdToBooking : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FloorId",
                table: "Bookings",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_FloorId",
                table: "Bookings",
                column: "FloorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Bookings_Floors_FloorId",
                table: "Bookings",
                column: "FloorId",
                principalTable: "Floors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bookings_Floors_FloorId",
                table: "Bookings");

            migrationBuilder.DropIndex(
                name: "IX_Bookings_FloorId",
                table: "Bookings");

            migrationBuilder.DropColumn(
                name: "FloorId",
                table: "Bookings");
        }
    }
}
