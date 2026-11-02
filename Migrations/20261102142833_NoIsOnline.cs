using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations;

/// <inheritdoc />
public partial class _20261102142833_NoIsOnline : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "IsOnline",
            table: "Users");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsOnline",
            table: "Users",
            type: "boolean",
            nullable: false,
            defaultValue: false);
    }
}
