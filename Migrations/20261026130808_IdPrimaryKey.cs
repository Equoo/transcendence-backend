using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KeepGrouped.Migrations;

/// <inheritdoc />
public partial class _20261026130808_IdPrimaryKey : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ChannelAcks_Users_UserId",
            table: "ChannelAcks");

        migrationBuilder.DropForeignKey(
            name: "FK_ChannelMembers_Users_UserId",
            table: "ChannelMembers");

        migrationBuilder.DropForeignKey(
            name: "FK_Events_Users_OrganizerEmail",
            table: "Events");

        migrationBuilder.DropForeignKey(
            name: "FK_Files_Users_CreatorEmail",
            table: "Files");

        migrationBuilder.DropForeignKey(
            name: "FK_Messages_Users_SenderId",
            table: "Messages");

        migrationBuilder.DropForeignKey(
            name: "FK_Registration_Users_UserEmail",
            table: "Registration");

        migrationBuilder.DropForeignKey(
            name: "FK_Relationships_Users_MeId",
            table: "Relationships");

        migrationBuilder.DropForeignKey(
            name: "FK_Relationships_Users_UserId",
            table: "Relationships");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Users",
            table: "Users");

        migrationBuilder.RenameColumn(
            name: "UserEmail",
            table: "Registration",
            newName: "UserId");

        migrationBuilder.RenameIndex(
            name: "IX_Registration_UserEmail",
            table: "Registration",
            newName: "IX_Registration_UserId");

        migrationBuilder.RenameColumn(
            name: "CreatorEmail",
            table: "Files",
            newName: "CreatorId");

        migrationBuilder.RenameIndex(
            name: "IX_Files_CreatorEmail",
            table: "Files",
            newName: "IX_Files_CreatorId");

        migrationBuilder.RenameColumn(
            name: "OrganizerEmail",
            table: "Events",
            newName: "OrganizerId");

        migrationBuilder.RenameIndex(
            name: "IX_Events_OrganizerEmail",
            table: "Events",
            newName: "IX_Events_OrganizerId");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Users",
            table: "Users",
            column: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_ChannelAcks_Users_UserId",
            table: "ChannelAcks",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_ChannelMembers_Users_UserId",
            table: "ChannelMembers",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Events_Users_OrganizerId",
            table: "Events",
            column: "OrganizerId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Files_Users_CreatorId",
            table: "Files",
            column: "CreatorId",
            principalTable: "Users",
            principalColumn: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Messages_Users_SenderId",
            table: "Messages",
            column: "SenderId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Registration_Users_UserId",
            table: "Registration",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Relationships_Users_MeId",
            table: "Relationships",
            column: "MeId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Relationships_Users_UserId",
            table: "Relationships",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_ChannelAcks_Users_UserId",
            table: "ChannelAcks");

        migrationBuilder.DropForeignKey(
            name: "FK_ChannelMembers_Users_UserId",
            table: "ChannelMembers");

        migrationBuilder.DropForeignKey(
            name: "FK_Events_Users_OrganizerId",
            table: "Events");

        migrationBuilder.DropForeignKey(
            name: "FK_Files_Users_CreatorId",
            table: "Files");

        migrationBuilder.DropForeignKey(
            name: "FK_Messages_Users_SenderId",
            table: "Messages");

        migrationBuilder.DropForeignKey(
            name: "FK_Registration_Users_UserId",
            table: "Registration");

        migrationBuilder.DropForeignKey(
            name: "FK_Relationships_Users_MeId",
            table: "Relationships");

        migrationBuilder.DropForeignKey(
            name: "FK_Relationships_Users_UserId",
            table: "Relationships");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Users",
            table: "Users");

        migrationBuilder.RenameColumn(
            name: "UserId",
            table: "Registration",
            newName: "UserEmail");

        migrationBuilder.RenameIndex(
            name: "IX_Registration_UserId",
            table: "Registration",
            newName: "IX_Registration_UserEmail");

        migrationBuilder.RenameColumn(
            name: "CreatorId",
            table: "Files",
            newName: "CreatorEmail");

        migrationBuilder.RenameIndex(
            name: "IX_Files_CreatorId",
            table: "Files",
            newName: "IX_Files_CreatorEmail");

        migrationBuilder.RenameColumn(
            name: "OrganizerId",
            table: "Events",
            newName: "OrganizerEmail");

        migrationBuilder.RenameIndex(
            name: "IX_Events_OrganizerId",
            table: "Events",
            newName: "IX_Events_OrganizerEmail");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Users",
            table: "Users",
            column: "Email");

        migrationBuilder.AddForeignKey(
            name: "FK_ChannelAcks_Users_UserId",
            table: "ChannelAcks",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Email",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_ChannelMembers_Users_UserId",
            table: "ChannelMembers",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Email",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Events_Users_OrganizerEmail",
            table: "Events",
            column: "OrganizerEmail",
            principalTable: "Users",
            principalColumn: "Email",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Files_Users_CreatorEmail",
            table: "Files",
            column: "CreatorEmail",
            principalTable: "Users",
            principalColumn: "Email");

        migrationBuilder.AddForeignKey(
            name: "FK_Messages_Users_SenderId",
            table: "Messages",
            column: "SenderId",
            principalTable: "Users",
            principalColumn: "Email",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Registration_Users_UserEmail",
            table: "Registration",
            column: "UserEmail",
            principalTable: "Users",
            principalColumn: "Email",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Relationships_Users_MeId",
            table: "Relationships",
            column: "MeId",
            principalTable: "Users",
            principalColumn: "Email",
            onDelete: ReferentialAction.Cascade);

        migrationBuilder.AddForeignKey(
            name: "FK_Relationships_Users_UserId",
            table: "Relationships",
            column: "UserId",
            principalTable: "Users",
            principalColumn: "Email",
            onDelete: ReferentialAction.Cascade);
    }
}
