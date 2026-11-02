using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations;

/// <inheritdoc />
public partial class _20261031194405_LowerMessageLength : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Content",
            table: "Messages",
            type: "character varying(2048)",
            maxLength: 2048,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(8192)",
            oldMaxLength: 8192);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AlterColumn<string>(
            name: "Content",
            table: "Messages",
            type: "character varying(8192)",
            maxLength: 8192,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(2048)",
            oldMaxLength: 2048);
    }
}
