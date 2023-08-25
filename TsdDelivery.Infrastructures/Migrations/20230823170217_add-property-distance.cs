using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TsdDelivery.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class addpropertydistance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Distance",
                table: "Reservation",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Distance",
                table: "Reservation");
        }
    }
}
