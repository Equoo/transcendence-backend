using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations
{
    /// <inheritdoc />
    public partial class EventRegistration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Role",
                table: "Registration");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "Registration",
                type: "text",
                nullable: true);

            migrationBuilder.AlterColumn<string[]>(
                name: "Tags",
                table: "Events",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0],
                oldClrType: typeof(string[]),
                oldType: "text[]",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "EventRoles",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventRoles", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Registration_RoleId",
                table: "Registration",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_EventRoles_RoleId",
                table: "Registration",
                column: "RoleId",
                principalTable: "EventRoles",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registration_EventRoles_RoleId",
                table: "Registration");

            migrationBuilder.DropTable(
                name: "EventRoles");

            migrationBuilder.DropIndex(
                name: "IX_Registration_RoleId",
                table: "Registration");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Registration");

            migrationBuilder.AddColumn<int>(
                name: "Role",
                table: "Registration",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string[]>(
                name: "Tags",
                table: "Events",
                type: "text[]",
                nullable: true,
                oldClrType: typeof(string[]),
                oldType: "text[]");

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Events",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
