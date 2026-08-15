using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations
{
    /// <inheritdoc />
    public partial class StructureRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registration_EventRoles_RoleId",
                table: "Registration");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "Registration",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_EventRoles_RoleId",
                table: "Registration",
                column: "RoleId",
                principalTable: "EventRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registration_EventRoles_RoleId",
                table: "Registration");

            migrationBuilder.AlterColumn<string>(
                name: "RoleId",
                table: "Registration",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_EventRoles_RoleId",
                table: "Registration",
                column: "RoleId",
                principalTable: "EventRoles",
                principalColumn: "Id");
        }
    }
}
