using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations;

/// <inheritdoc />
public partial class _20261029151559_EventChannelRelation : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Events_Channels_ChannelId",
            table: "Events");

        migrationBuilder.DropIndex(
            name: "IX_Events_ChannelId",
            table: "Events");

        migrationBuilder.DropColumn(
            name: "ChannelId",
            table: "Events");

        migrationBuilder.CreateIndex(
            name: "IX_Channels_EventId",
            table: "Channels",
            column: "EventId",
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "FK_Channels_Events_EventId",
            table: "Channels",
            column: "EventId",
            principalTable: "Events",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Channels_Events_EventId",
            table: "Channels");

        migrationBuilder.DropIndex(
            name: "IX_Channels_EventId",
            table: "Channels");

        migrationBuilder.AddColumn<string>(
            name: "ChannelId",
            table: "Events",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.CreateIndex(
            name: "IX_Events_ChannelId",
            table: "Events",
            column: "ChannelId",
            unique: true);

        migrationBuilder.AddForeignKey(
            name: "FK_Events_Channels_ChannelId",
            table: "Events",
            column: "ChannelId",
            principalTable: "Channels",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
