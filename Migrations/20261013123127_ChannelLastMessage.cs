using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations
{
    /// <inheritdoc />
    public partial class ChannelLastMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastMessageId",
                table: "Channels",
                type: "character varying(36)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Channels_LastMessageId",
                table: "Channels",
                column: "LastMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Channels_Messages_LastMessageId",
                table: "Channels",
                column: "LastMessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Channels_Messages_LastMessageId",
                table: "Channels");

            migrationBuilder.DropIndex(
                name: "IX_Channels_LastMessageId",
                table: "Channels");

            migrationBuilder.DropColumn(
                name: "LastMessageId",
                table: "Channels");
        }
    }
}
