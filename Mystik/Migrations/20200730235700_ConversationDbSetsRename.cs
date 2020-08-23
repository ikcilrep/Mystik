using Microsoft.EntityFrameworkCore.Migrations;

namespace Mystik.Migrations
{
    public partial class ConversationDbSetsRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagedConversations_Conversations_ConversationId",
                table: "ManagedConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_ManagedConversations_Users_ManagerId",
                table: "ManagedConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConversations_Conversations_ConversationId",
                table: "UserConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConversations_Users_UserId",
                table: "UserConversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConversations",
                table: "UserConversations");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ManagedConversations",
                table: "ManagedConversations");

            migrationBuilder.RenameTable(
                name: "UserConversations",
                newName: "ConversationMembers");

            migrationBuilder.RenameTable(
                name: "ManagedConversations",
                newName: "ConversationManagers");

            migrationBuilder.RenameIndex(
                name: "IX_UserConversations_ConversationId",
                table: "ConversationMembers",
                newName: "IX_ConversationMembers_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_ManagedConversations_ConversationId",
                table: "ConversationManagers",
                newName: "IX_ConversationManagers_ConversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationMembers",
                table: "ConversationMembers",
                columns: new[] { "UserId", "ConversationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ConversationManagers",
                table: "ConversationManagers",
                columns: new[] { "ManagerId", "ConversationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationManagers_Conversations_ConversationId",
                table: "ConversationManagers",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationManagers_Users_ManagerId",
                table: "ConversationManagers",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMembers_Conversations_ConversationId",
                table: "ConversationMembers",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ConversationMembers_Users_UserId",
                table: "ConversationMembers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConversationManagers_Conversations_ConversationId",
                table: "ConversationManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationManagers_Users_ManagerId",
                table: "ConversationManagers");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMembers_Conversations_ConversationId",
                table: "ConversationMembers");

            migrationBuilder.DropForeignKey(
                name: "FK_ConversationMembers_Users_UserId",
                table: "ConversationMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationMembers",
                table: "ConversationMembers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ConversationManagers",
                table: "ConversationManagers");

            migrationBuilder.RenameTable(
                name: "ConversationMembers",
                newName: "UserConversations");

            migrationBuilder.RenameTable(
                name: "ConversationManagers",
                newName: "ManagedConversations");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationMembers_ConversationId",
                table: "UserConversations",
                newName: "IX_UserConversations_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_ConversationManagers_ConversationId",
                table: "ManagedConversations",
                newName: "IX_ManagedConversations_ConversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConversations",
                table: "UserConversations",
                columns: new[] { "UserId", "ConversationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ManagedConversations",
                table: "ManagedConversations",
                columns: new[] { "ManagerId", "ConversationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ManagedConversations_Conversations_ConversationId",
                table: "ManagedConversations",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagedConversations_Users_ManagerId",
                table: "ManagedConversations",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserConversations_Conversations_ConversationId",
                table: "UserConversations",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserConversations_Users_UserId",
                table: "UserConversations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
