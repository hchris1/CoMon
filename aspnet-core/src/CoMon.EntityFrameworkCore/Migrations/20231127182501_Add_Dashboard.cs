using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Add_Dashboard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoMonDashboards",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoMonDashboards", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoMonDashboardTiles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SortIndex = table.Column<int>(type: "integer", nullable: false),
                    ItemType = table.Column<int>(type: "integer", nullable: false),
                    ItemId = table.Column<long>(type: "bigint", nullable: false),
                    DashboardId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoMonDashboardTiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoMonDashboardTiles_CoMonDashboards_DashboardId",
                        column: x => x.DashboardId,
                        principalTable: "CoMonDashboards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoMonDashboardTiles_DashboardId",
                table: "CoMonDashboardTiles",
                column: "DashboardId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoMonDashboardTiles");

            migrationBuilder.DropTable(
                name: "CoMonDashboards");
        }
    }
}
