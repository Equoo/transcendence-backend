using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations
{
    /// <inheritdoc />
    public partial class AddChannelCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChannelCategories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Order = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelCategories", x => x.Id);
                    table.UniqueConstraint("AK_ChannelCategories_Name", x => x.Name);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Channels_Category",
                table: "Channels",
                column: "Category");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_ChannelCategories_Category",
                table: "Channels",
                column: "Category",
                principalTable: "ChannelCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_ChannelCategories_Category",
                table: "Channels");

            migrationBuilder.DropTable(
                name: "ChannelCategories");

            migrationBuilder.DropIndex(
                name: "IX_Channels_Category",
                table: "Channels");
        }
    }
}
