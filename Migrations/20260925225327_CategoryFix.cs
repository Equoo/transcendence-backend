using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations
{
    /// <inheritdoc />
    public partial class CategoryFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChannelRoles",
                table: "ChannelRoles");

            migrationBuilder.AddColumn<string>(
                name: "CategoryId",
                table: "ChannelRoles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelRoles",
                table: "ChannelRoles",
                columns: new[] { "RoleId", "CategoryId" });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelRoles_CategoryId",
                table: "ChannelRoles",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_ChannelRoles_ChannelCategories_CategoryId",
                table: "ChannelRoles",
                column: "CategoryId",
                principalTable: "ChannelCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChannelRoles_ChannelCategories_CategoryId",
                table: "ChannelRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ChannelRoles",
                table: "ChannelRoles");

            migrationBuilder.DropIndex(
                name: "IX_ChannelRoles_CategoryId",
                table: "ChannelRoles");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ChannelRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelRoles",
                table: "ChannelRoles",
                columns: new[] { "RoleId", "ChannelId" });
        }
    }
}
