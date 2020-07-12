using Microsoft.EntityFrameworkCore.Migrations;

namespace Mystik.Migrations
{
    public partial class RemoveEncryptedContentPath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedContentPath",
                table: "Messages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncryptedContentPath",
                table: "Messages",
                type: "text",
                nullable: true);
        }
    }
}
