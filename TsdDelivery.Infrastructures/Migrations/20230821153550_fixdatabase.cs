using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TsdDelivery.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class fixdatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Users_DriverId1",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_DriverId1",
                table: "Reservation");

            migrationBuilder.DropColumn(
                name: "DriverId1",
                table: "Reservation");

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_UserId",
                table: "Reservation",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Users_UserId",
                table: "Reservation",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reservation_Users_UserId",
                table: "Reservation");

            migrationBuilder.DropIndex(
                name: "IX_Reservation_UserId",
                table: "Reservation");

            migrationBuilder.AddColumn<Guid>(
                name: "DriverId1",
                table: "Reservation",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Reservation_DriverId1",
                table: "Reservation",
                column: "DriverId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Reservation_Users_DriverId1",
                table: "Reservation",
                column: "DriverId1",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
