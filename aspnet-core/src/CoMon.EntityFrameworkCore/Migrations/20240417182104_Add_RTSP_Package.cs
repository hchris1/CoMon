using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Add_RTSP_Package : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RtspPackageSettingsId",
                table: "CoMonPackages",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CoMonRtspPackageSettings",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Url = table.Column<string>(type: "TEXT", maxLength: 255, nullable: false),
                    CycleSeconds = table.Column<int>(type: "INTEGER", nullable: false),
                    Method = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoMonRtspPackageSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoMonPackages_RtspPackageSettingsId",
                table: "CoMonPackages",
                column: "RtspPackageSettingsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonPackages_CoMonRtspPackageSettings_RtspPackageSettingsId",
                table: "CoMonPackages",
                column: "RtspPackageSettingsId",
                principalTable: "CoMonRtspPackageSettings",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoMonPackages_CoMonRtspPackageSettings_RtspPackageSettingsId",
                table: "CoMonPackages");

            migrationBuilder.DropTable(
                name: "CoMonRtspPackageSettings");

            migrationBuilder.DropIndex(
                name: "IX_CoMonPackages_RtspPackageSettingsId",
                table: "CoMonPackages");

            migrationBuilder.DropColumn(
                name: "RtspPackageSettingsId",
                table: "CoMonPackages");
        }
    }
}
