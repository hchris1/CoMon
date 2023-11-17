using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Image_Auditing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "CoMonImages");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "CoMonImages");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "CoMonImages");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "CoMonImages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CoMonImages");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "CoMonImages");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "CoMonImages");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "CoMonImages",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "CoMonImages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "CoMonImages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "CoMonImages",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CoMonImages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "CoMonImages",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "CoMonImages",
                type: "bigint",
                nullable: true);
        }
    }
}
