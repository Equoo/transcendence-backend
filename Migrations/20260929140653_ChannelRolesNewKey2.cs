using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations
{
    /// <inheritdoc />
    public partial class ChannelRolesNewKey2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChannelRoles",
                table: "ChannelRoles");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ChannelRoles",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelRoles",
                table: "ChannelRoles",
                columns: new[] { "RoleId", "Id" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ChannelRoles",
                table: "ChannelRoles");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ChannelRoles");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ChannelRoles",
                table: "ChannelRoles",
                column: "RoleId");
        }
    }
}
