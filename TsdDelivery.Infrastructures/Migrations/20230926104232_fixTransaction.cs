﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TsdDelivery.Infrastructures.Migrations
{
    /// <inheritdoc />
    public partial class fixTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Transaction");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Transaction",
                type: "datetime2",
                nullable: true);
        }
    }
}
