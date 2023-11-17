using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Rename_Package_Settings_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoMonPackages_PingPackageSettings_PingPackageSettingsId",
                table: "CoMonPackages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PingPackageSettings",
                table: "PingPackageSettings");

            migrationBuilder.RenameTable(
                name: "PingPackageSettings",
                newName: "CoMonPingPackageSettings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoMonPingPackageSettings",
                table: "CoMonPingPackageSettings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonPackages_CoMonPingPackageSettings_PingPackageSettingsId",
                table: "CoMonPackages",
                column: "PingPackageSettingsId",
                principalTable: "CoMonPingPackageSettings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoMonPackages_CoMonPingPackageSettings_PingPackageSettingsId",
                table: "CoMonPackages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoMonPingPackageSettings",
                table: "CoMonPingPackageSettings");

            migrationBuilder.RenameTable(
                name: "CoMonPingPackageSettings",
                newName: "PingPackageSettings");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PingPackageSettings",
                table: "PingPackageSettings",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonPackages_PingPackageSettings_PingPackageSettingsId",
                table: "CoMonPackages",
                column: "PingPackageSettingsId",
                principalTable: "PingPackageSettings",
                principalColumn: "Id");
        }
    }
}
