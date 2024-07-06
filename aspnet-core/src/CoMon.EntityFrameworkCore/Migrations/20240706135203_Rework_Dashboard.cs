using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Rework_Dashboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SortIndex",
                table: "CoMonDashboardTiles",
                newName: "Y");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "CoMonDashboardTiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "CoMonDashboardTiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "X",
                table: "CoMonDashboardTiles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Height",
                table: "CoMonDashboardTiles");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "CoMonDashboardTiles");

            migrationBuilder.DropColumn(
                name: "X",
                table: "CoMonDashboardTiles");

            migrationBuilder.RenameColumn(
                name: "Y",
                table: "CoMonDashboardTiles",
                newName: "SortIndex");
        }
    }
}
