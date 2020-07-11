using Microsoft.EntityFrameworkCore.Migrations;

namespace Mystik.Migrations
{
    public partial class ConversationUserRelations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagedConversation_Conversations_ConversationId",
                table: "ManagedConversation");

            migrationBuilder.DropForeignKey(
                name: "FK_ManagedConversation_Users_AdminId",
                table: "ManagedConversation");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConversation_Conversations_ConversationId",
                table: "UserConversation");

            migrationBuilder.DropForeignKey(
                name: "FK_UserConversation_Users_UserId",
                table: "UserConversation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserConversation",
                table: "UserConversation");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ManagedConversation",
                table: "ManagedConversation");

            migrationBuilder.RenameTable(
                name: "UserConversation",
                newName: "UserConversations");

            migrationBuilder.RenameTable(
                name: "ManagedConversation",
                newName: "ManagedConversations");

            migrationBuilder.RenameIndex(
                name: "IX_UserConversation_ConversationId",
                table: "UserConversations",
                newName: "IX_UserConversations_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_ManagedConversation_ConversationId",
                table: "ManagedConversations",
                newName: "IX_ManagedConversations_ConversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConversations",
                table: "UserConversations",
                columns: new[] { "UserId", "ConversationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ManagedConversations",
                table: "ManagedConversations",
                columns: new[] { "AdminId", "ConversationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ManagedConversations_Conversations_ConversationId",
                table: "ManagedConversations",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagedConversations_Users_AdminId",
                table: "ManagedConversations",
                column: "AdminId",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagedConversations_Conversations_ConversationId",
                table: "ManagedConversations");

            migrationBuilder.DropForeignKey(
                name: "FK_ManagedConversations_Users_AdminId",
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
                newName: "UserConversation");

            migrationBuilder.RenameTable(
                name: "ManagedConversations",
                newName: "ManagedConversation");

            migrationBuilder.RenameIndex(
                name: "IX_UserConversations_ConversationId",
                table: "UserConversation",
                newName: "IX_UserConversation_ConversationId");

            migrationBuilder.RenameIndex(
                name: "IX_ManagedConversations_ConversationId",
                table: "ManagedConversation",
                newName: "IX_ManagedConversation_ConversationId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserConversation",
                table: "UserConversation",
                columns: new[] { "UserId", "ConversationId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_ManagedConversation",
                table: "ManagedConversation",
                columns: new[] { "AdminId", "ConversationId" });

            migrationBuilder.AddForeignKey(
                name: "FK_ManagedConversation_Conversations_ConversationId",
                table: "ManagedConversation",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ManagedConversation_Users_AdminId",
                table: "ManagedConversation",
                column: "AdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserConversation_Conversations_ConversationId",
                table: "UserConversation",
                column: "ConversationId",
                principalTable: "Conversations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserConversation_Users_UserId",
                table: "UserConversation",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
