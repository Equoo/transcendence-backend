using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations
{
    /// <inheritdoc />
    public partial class ChannelRolesNewKey3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChannelRoles",
                table: "ChannelRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelRoles",
                table: "ChannelRoles",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ChannelRoles_RoleId",
                table: "ChannelRoles",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChannelRoles",
                table: "ChannelRoles");

            migrationBuilder.DropIndex(
                name: "IX_ChannelRoles_RoleId",
                table: "ChannelRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelRoles",
                table: "ChannelRoles",
                columns: new[] { "RoleId", "Id" });
        }
    }
}
