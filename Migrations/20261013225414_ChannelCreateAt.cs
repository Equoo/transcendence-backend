using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations
{
    /// <inheritdoc />
    public partial class ChannelCreateAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreateAt",
                table: "Channels",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "now()");

            // Existing channels: best guess is their first message
            migrationBuilder.Sql("""
                UPDATE "Channels" c
                SET "CreateAt" = m."FirstSentAt"
                FROM (
                    SELECT "ChannelId", MIN("SentAt") AS "FirstSentAt"
                    FROM "Messages"
                    GROUP BY "ChannelId"
                ) m
                WHERE m."ChannelId" = c."Id";
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateAt",
                table: "Channels");
        }
    }
}
