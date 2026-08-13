using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations
{
    /// <inheritdoc />
    public partial class RenameFileToStorageFile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventStorageFile",
                columns: table => new
                {
                    EventsId = table.Column<string>(type: "text", nullable: false),
                    FilesKey = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventStorageFile", x => new { x.EventsId, x.FilesKey });
                    table.ForeignKey(
                        name: "FK_EventStorageFile_Events_EventsId",
                        column: x => x.EventsId,
                        principalTable: "Events",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EventStorageFile_Files_FilesKey",
                        column: x => x.FilesKey,
                        principalTable: "Files",
                        principalColumn: "Key",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EventStorageFile_FilesKey",
                table: "EventStorageFile",
                column: "FilesKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventStorageFile");
        }
    }
}
