using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Add_Content_To_Dashboard_Tiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "CoMonDashboardTiles",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "CoMonDashboardTiles");
        }
    }
}
