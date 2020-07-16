using Microsoft.EntityFrameworkCore.Migrations;

namespace Mystik.Migrations
{
    public partial class AdminToManager : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagedConversations_Users_AdminId",
                table: "ManagedConversations");

            migrationBuilder.RenameColumn(
                name: "AdminId",
                table: "ManagedConversations",
                newName: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ManagedConversations_Users_ManagerId",
                table: "ManagedConversations",
                column: "ManagerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ManagedConversations_Users_ManagerId",
                table: "ManagedConversations");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "ManagedConversations",
                newName: "AdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_ManagedConversations_Users_AdminId",
                table: "ManagedConversations",
                column: "AdminId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
