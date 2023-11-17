using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Add_Charts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CoMonCharts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(type: "text", nullable: true),
                    SubTitle = table.Column<string>(type: "text", nullable: true),
                    Labels = table.Column<List<string>>(type: "text[]", nullable: true),
                    Criticality = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoMonCharts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CoMonSeries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    VizType = table.Column<int>(type: "integer", nullable: false),
                    XUnit = table.Column<string>(type: "text", nullable: true),
                    YUnit = table.Column<string>(type: "text", nullable: true),
                    ChartId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoMonSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoMonSeries_CoMonCharts_ChartId",
                        column: x => x.ChartId,
                        principalTable: "CoMonCharts",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CoMonDataPoints",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    Tag = table.Column<string>(type: "text", nullable: true),
                    X = table.Column<double>(type: "double precision", nullable: true),
                    Y = table.Column<List<double>>(type: "double precision[]", nullable: true),
                    SeriesId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoMonDataPoints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoMonDataPoints_CoMonSeries_SeriesId",
                        column: x => x.SeriesId,
                        principalTable: "CoMonSeries",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoMonDataPoints_SeriesId",
                table: "CoMonDataPoints",
                column: "SeriesId");

            migrationBuilder.CreateIndex(
                name: "IX_CoMonSeries_ChartId",
                table: "CoMonSeries",
                column: "ChartId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CoMonDataPoints");

            migrationBuilder.DropTable(
                name: "CoMonSeries");

            migrationBuilder.DropTable(
                name: "CoMonCharts");
        }
    }
}
