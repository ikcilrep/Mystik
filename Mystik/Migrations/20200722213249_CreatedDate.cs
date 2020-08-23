using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Mystik.Migrations
{
    public partial class CreatedDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SentTime",
                table: "Messages",
                newName: "CreatedDate");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "ManagedConversations",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Invitations",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Friends",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "ManagedConversations");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Friends");

            migrationBuilder.RenameColumn(
                name: "CreatedDate",
                table: "Messages",
                newName: "SentTime");
        }
    }
}
