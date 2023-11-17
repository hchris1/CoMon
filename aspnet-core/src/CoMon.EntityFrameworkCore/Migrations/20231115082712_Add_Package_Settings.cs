using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Add_Package_Settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "Guid",
                table: "CoMonPackages",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<long>(
                name: "PingPackageSettingsId",
                table: "CoMonPackages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PingPackageSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Host = table.Column<string>(type: "text", nullable: true),
                    CycleSeconds = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PingPackageSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoMonPackages_PingPackageSettingsId",
                table: "CoMonPackages",
                column: "PingPackageSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonPackages_PingPackageSettings_PingPackageSettingsId",
                table: "CoMonPackages",
                column: "PingPackageSettingsId",
                principalTable: "PingPackageSettings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoMonPackages_PingPackageSettings_PingPackageSettingsId",
                table: "CoMonPackages");

            migrationBuilder.DropTable(
                name: "PingPackageSettings");

            migrationBuilder.DropIndex(
                name: "IX_CoMonPackages_PingPackageSettingsId",
                table: "CoMonPackages");

            migrationBuilder.DropColumn(
                name: "Guid",
                table: "CoMonPackages");

            migrationBuilder.DropColumn(
                name: "PingPackageSettingsId",
                table: "CoMonPackages");
        }
    }
}
