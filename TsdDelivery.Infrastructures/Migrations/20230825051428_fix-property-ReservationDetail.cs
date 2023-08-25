using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TsdDelivery.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class fixpropertyReservationDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CreatedBy",
                table: "ReservationDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationDate",
                table: "ReservationDetail",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<Guid>(
                name: "DeleteBy",
                table: "ReservationDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionDate",
                table: "ReservationDetail",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "ReservationDetail",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "ModificationBy",
                table: "ReservationDetail",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModificationDate",
                table: "ReservationDetail",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "ReservationDetail");

            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "ReservationDetail");

            migrationBuilder.DropColumn(
                name: "DeleteBy",
                table: "ReservationDetail");

            migrationBuilder.DropColumn(
                name: "DeletionDate",
                table: "ReservationDetail");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "ReservationDetail");

            migrationBuilder.DropColumn(
                name: "ModificationBy",
                table: "ReservationDetail");

            migrationBuilder.DropColumn(
                name: "ModificationDate",
                table: "ReservationDetail");
        }
    }
}
