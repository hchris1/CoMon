using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Add_Http_Package_Settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "HttpPackageSettingsId",
                table: "CoMonPackages",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CoMonHttpPackageSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Url = table.Column<string>(type: "text", nullable: false),
                    CycleSeconds = table.Column<int>(type: "integer", nullable: false),
                    Method = table.Column<int>(type: "integer", nullable: false),
                    Headers = table.Column<string>(type: "text", nullable: false),
                    Body = table.Column<string>(type: "text", nullable: false),
                    Encoding = table.Column<int>(type: "integer", nullable: false),
                    IgnoreSslErrors = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoMonHttpPackageSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoMonPackages_HttpPackageSettingsId",
                table: "CoMonPackages",
                column: "HttpPackageSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonPackages_CoMonHttpPackageSettings_HttpPackageSettingsId",
                table: "CoMonPackages",
                column: "HttpPackageSettingsId",
                principalTable: "CoMonHttpPackageSettings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoMonPackages_CoMonHttpPackageSettings_HttpPackageSettingsId",
                table: "CoMonPackages");

            migrationBuilder.DropTable(
                name: "CoMonHttpPackageSettings");

            migrationBuilder.DropIndex(
                name: "IX_CoMonPackages_HttpPackageSettingsId",
                table: "CoMonPackages");

            migrationBuilder.DropColumn(
                name: "HttpPackageSettingsId",
                table: "CoMonPackages");
        }
    }
}
