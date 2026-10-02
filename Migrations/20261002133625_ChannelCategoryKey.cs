using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations
{
    /// <inheritdoc />
    public partial class ChannelCategoryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_ChannelCategories_Category",
                table: "Channels");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Channels",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Channels_Category",
                table: "Channels",
                newName: "IX_Channels_CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_ChannelCategories_CategoryId",
                table: "Channels",
                column: "CategoryId",
                principalTable: "ChannelCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_ChannelCategories_CategoryId",
                table: "Channels");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Channels",
                newName: "Category");

            migrationBuilder.RenameIndex(
                name: "IX_Channels_CategoryId",
                table: "Channels",
                newName: "IX_Channels_Category");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_ChannelCategories_Category",
                table: "Channels",
                column: "Category",
                principalTable: "ChannelCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
