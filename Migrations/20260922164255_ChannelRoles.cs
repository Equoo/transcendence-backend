using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations
{
    /// <inheritdoc />
    public partial class ChannelRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "CategorySync",
                table: "Channels",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "ChannelRoles",
                columns: table => new
                {
                    ChannelId = table.Column<string>(type: "text", nullable: false),
                    RoleId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChannelRoles", x => new { x.RoleId, x.ChannelId });
                    table.ForeignKey(
                        name: "FK_ChannelRoles_Channels_ChannelId",
                        column: x => x.ChannelId,
                        principalTable: "Channels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ChannelRoles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ChannelRoles_ChannelId",
                table: "ChannelRoles",
                column: "ChannelId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChannelRoles");

            migrationBuilder.DropColumn(
                name: "CategorySync",
                table: "Channels");
        }
    }
}
