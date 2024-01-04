using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Auditing_From_Group : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "CoMonGroups");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "CoMonGroups");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "CoMonGroups");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "CoMonGroups");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CoMonGroups");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "CoMonGroups");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "CoMonGroups");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "CoMonGroups",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "CoMonGroups",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "CoMonGroups",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "CoMonGroups",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CoMonGroups",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "CoMonGroups",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "CoMonGroups",
                type: "INTEGER",
                nullable: true);
        }
    }
}
