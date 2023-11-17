using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoMon.Migrations
{
    /// <inheritdoc />
    public partial class Rename_Groups_Table : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoMonAssets_Groups_GroupId",
                table: "CoMonAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_Groups_Groups_GroupId",
                table: "Groups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Groups",
                table: "Groups");

            migrationBuilder.RenameTable(
                name: "Groups",
                newName: "CoMonGroups");

            migrationBuilder.RenameColumn(
                name: "GroupId",
                table: "CoMonGroups",
                newName: "Group");

            migrationBuilder.RenameIndex(
                name: "IX_Groups_GroupId",
                table: "CoMonGroups",
                newName: "IX_CoMonGroups_Group");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CoMonGroups",
                table: "CoMonGroups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonAssets_CoMonGroups_GroupId",
                table: "CoMonAssets",
                column: "GroupId",
                principalTable: "CoMonGroups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonGroups_CoMonGroups_Group",
                table: "CoMonGroups",
                column: "Group",
                principalTable: "CoMonGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CoMonAssets_CoMonGroups_GroupId",
                table: "CoMonAssets");

            migrationBuilder.DropForeignKey(
                name: "FK_CoMonGroups_CoMonGroups_Group",
                table: "CoMonGroups");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CoMonGroups",
                table: "CoMonGroups");

            migrationBuilder.RenameTable(
                name: "CoMonGroups",
                newName: "Groups");

            migrationBuilder.RenameColumn(
                name: "Group",
                table: "Groups",
                newName: "GroupId");

            migrationBuilder.RenameIndex(
                name: "IX_CoMonGroups_Group",
                table: "Groups",
                newName: "IX_Groups_GroupId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Groups",
                table: "Groups",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CoMonAssets_Groups_GroupId",
                table: "CoMonAssets",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Groups_Groups_GroupId",
                table: "Groups",
                column: "GroupId",
                principalTable: "Groups",
                principalColumn: "Id");
        }
    }
}
