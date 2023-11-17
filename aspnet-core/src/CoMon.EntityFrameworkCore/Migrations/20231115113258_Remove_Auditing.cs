using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Auditing : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "CoMonPackages");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "CoMonPackages");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "CoMonPackages");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "CoMonPackages");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CoMonPackages");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "CoMonPackages");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "CoMonPackages");

            migrationBuilder.DropColumn(
                name: "CreationTime",
                table: "CoMonAssets");

            migrationBuilder.DropColumn(
                name: "CreatorUserId",
                table: "CoMonAssets");

            migrationBuilder.DropColumn(
                name: "DeleterUserId",
                table: "CoMonAssets");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "CoMonAssets");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "CoMonAssets");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "CoMonAssets");

            migrationBuilder.DropColumn(
                name: "LastModifierUserId",
                table: "CoMonAssets");

            migrationBuilder.AlterColumn<string>(
                name: "Host",
                table: "CoMonPingPackageSettings",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Host",
                table: "CoMonPingPackageSettings",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "CoMonPackages",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "CoMonPackages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "CoMonPackages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "CoMonPackages",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CoMonPackages",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "CoMonPackages",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "CoMonPackages",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreationTime",
                table: "CoMonAssets",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<long>(
                name: "CreatorUserId",
                table: "CoMonAssets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DeleterUserId",
                table: "CoMonAssets",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "CoMonAssets",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "CoMonAssets",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "CoMonAssets",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "LastModifierUserId",
                table: "CoMonAssets",
                type: "bigint",
                nullable: true);
        }
    }
}
