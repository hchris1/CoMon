using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Attach_Charts_To_Status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "StatusId",
                table: "CoMonCharts",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CoMonCharts_StatusId",
                table: "CoMonCharts",
                column: "StatusId");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonCharts_CoMonStatuses_StatusId",
                table: "CoMonCharts",
                column: "StatusId",
                principalTable: "CoMonStatuses",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoMonCharts_CoMonStatuses_StatusId",
                table: "CoMonCharts");

            migrationBuilder.DropIndex(
                name: "IX_CoMonCharts_StatusId",
                table: "CoMonCharts");

            migrationBuilder.DropColumn(
                name: "StatusId",
                table: "CoMonCharts");
        }
    }
}
