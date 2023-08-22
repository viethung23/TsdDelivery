using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TsdDelivery.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class addondeleteCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRate_Service_ServiceId",
                table: "ShippingRate");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRate_Service_ServiceId",
                table: "ShippingRate",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShippingRate_Service_ServiceId",
                table: "ShippingRate");

            migrationBuilder.AddForeignKey(
                name: "FK_ShippingRate_Service_ServiceId",
                table: "ShippingRate",
                column: "ServiceId",
                principalTable: "Service",
                principalColumn: "Id");
        }
    }
}
