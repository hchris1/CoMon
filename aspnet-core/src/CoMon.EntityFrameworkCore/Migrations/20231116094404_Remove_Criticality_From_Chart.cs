using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Remove_Criticality_From_Chart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoMonCharts_CoMonStatuses_StatusId",
                table: "CoMonCharts");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonDataPoints_CoMonSeries_SeriesId",
                table: "CoMonDataPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonImages_CoMonAssets_AssetId",
                table: "CoMonImages");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonKPIs_CoMonStatuses_StatusId",
                table: "CoMonKPIs");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonPackages_CoMonAssets_AssetId",
                table: "CoMonPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonSeries_CoMonCharts_ChartId",
                table: "CoMonSeries");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonStatuses_CoMonPackages_PackageId",
                table: "CoMonStatuses");

            migrationBuilder.DropColumn(
                name: "Criticality",
                table: "CoMonCharts");

            migrationBuilder.AlterColumn<long>(
                name: "PackageId",
                table: "CoMonStatuses",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "ChartId",
                table: "CoMonSeries",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AssetId",
                table: "CoMonPackages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "StatusId",
                table: "CoMonKPIs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "AssetId",
                table: "CoMonImages",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "SeriesId",
                table: "CoMonDataPoints",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "StatusId",
                table: "CoMonCharts",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonCharts_CoMonStatuses_StatusId",
                table: "CoMonCharts",
                column: "StatusId",
                principalTable: "CoMonStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonDataPoints_CoMonSeries_SeriesId",
                table: "CoMonDataPoints",
                column: "SeriesId",
                principalTable: "CoMonSeries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonImages_CoMonAssets_AssetId",
                table: "CoMonImages",
                column: "AssetId",
                principalTable: "CoMonAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonKPIs_CoMonStatuses_StatusId",
                table: "CoMonKPIs",
                column: "StatusId",
                principalTable: "CoMonStatuses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonPackages_CoMonAssets_AssetId",
                table: "CoMonPackages",
                column: "AssetId",
                principalTable: "CoMonAssets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonSeries_CoMonCharts_ChartId",
                table: "CoMonSeries",
                column: "ChartId",
                principalTable: "CoMonCharts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonStatuses_CoMonPackages_PackageId",
                table: "CoMonStatuses",
                column: "PackageId",
                principalTable: "CoMonPackages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoMonCharts_CoMonStatuses_StatusId",
                table: "CoMonCharts");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonDataPoints_CoMonSeries_SeriesId",
                table: "CoMonDataPoints");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonImages_CoMonAssets_AssetId",
                table: "CoMonImages");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonKPIs_CoMonStatuses_StatusId",
                table: "CoMonKPIs");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonPackages_CoMonAssets_AssetId",
                table: "CoMonPackages");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonSeries_CoMonCharts_ChartId",
                table: "CoMonSeries");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonStatuses_CoMonPackages_PackageId",
                table: "CoMonStatuses");

            migrationBuilder.AlterColumn<long>(
                name: "PackageId",
                table: "CoMonStatuses",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "ChartId",
                table: "CoMonSeries",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "AssetId",
                table: "CoMonPackages",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "StatusId",
                table: "CoMonKPIs",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "AssetId",
                table: "CoMonImages",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "SeriesId",
                table: "CoMonDataPoints",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<long>(
                name: "StatusId",
                table: "CoMonCharts",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<int>(
                name: "Criticality",
                table: "CoMonCharts",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonCharts_CoMonStatuses_StatusId",
                table: "CoMonCharts",
                column: "StatusId",
                principalTable: "CoMonStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonDataPoints_CoMonSeries_SeriesId",
                table: "CoMonDataPoints",
                column: "SeriesId",
                principalTable: "CoMonSeries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonImages_CoMonAssets_AssetId",
                table: "CoMonImages",
                column: "AssetId",
                principalTable: "CoMonAssets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonKPIs_CoMonStatuses_StatusId",
                table: "CoMonKPIs",
                column: "StatusId",
                principalTable: "CoMonStatuses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonPackages_CoMonAssets_AssetId",
                table: "CoMonPackages",
                column: "AssetId",
                principalTable: "CoMonAssets",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonSeries_CoMonCharts_ChartId",
                table: "CoMonSeries",
                column: "ChartId",
                principalTable: "CoMonCharts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonStatuses_CoMonPackages_PackageId",
                table: "CoMonStatuses",
                column: "PackageId",
                principalTable: "CoMonPackages",
                principalColumn: "Id");
        }
    }
}
