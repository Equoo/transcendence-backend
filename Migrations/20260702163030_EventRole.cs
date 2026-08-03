using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations
{
    /// <inheritdoc />
    public partial class EventRole : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "AK_EventRoles_Name",
                table: "EventRoles",
                column: "Name");

            migrationBuilder.CreateTable(
                name: "EventEventRole",
                columns: table => new
                {
                    EventRolesId = table.Column<string>(type: "text", nullable: false),
                    EventsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventEventRole", x => new { x.EventRolesId, x.EventsId });
                    table.ForeignKey(
                        name: "FK_EventEventRole_EventRoles_EventRolesId",
                        column: x => x.EventRolesId,
                        principalTable: "EventRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventEventRole_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventEventRole_EventsId",
                table: "EventEventRole",
                column: "EventsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventEventRole");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_EventRoles_Name",
                table: "EventRoles");
        }
    }
}
