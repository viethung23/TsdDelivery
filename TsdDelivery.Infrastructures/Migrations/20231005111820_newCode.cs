using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TsdDelivery.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class newCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Code",
                table: "Reservation",
                type: "bigint",
                nullable: false,
                defaultValue: 0L)
                .Annotation("SqlServer:Identity", "1000, 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "Reservation");
        }
    }
}
