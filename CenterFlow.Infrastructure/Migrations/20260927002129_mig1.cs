using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CenterFlow.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class mig1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentBookings",
                table: "StudentBookings");

            migrationBuilder.DropColumn(
                name: "StudentCount",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "DayOfWeek",
                table: "Books",
                newName: "Date");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "StudentBookings",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsCancelled",
                table: "StudentBookings",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentBookings",
                table: "StudentBookings",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_StudentBookings_BookId",
                table: "StudentBookings",
                column: "BookId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_StudentBookings",
                table: "StudentBookings");

            migrationBuilder.DropIndex(
                name: "IX_StudentBookings_BookId",
                table: "StudentBookings");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "StudentBookings");

            migrationBuilder.DropColumn(
                name: "IsCancelled",
                table: "StudentBookings");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Books",
                newName: "DayOfWeek");

            migrationBuilder.AddColumn<int>(
                name: "StudentCount",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudentBookings",
                table: "StudentBookings",
                columns: new[] { "BookId", "StudentId" });
        }
    }
}
