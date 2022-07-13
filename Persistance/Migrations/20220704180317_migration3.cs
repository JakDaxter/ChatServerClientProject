using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    public partial class migration3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ToUser",
                table: "FriendRequests",
                newName: "ToIdUser");

            migrationBuilder.RenameColumn(
                name: "FromUser",
                table: "FriendRequests",
                newName: "FromIdUser");

            migrationBuilder.CreateIndex(
                name: "IX_FriendRequests_FromIdUser_ToIdUser",
                table: "FriendRequests",
                columns: new[] { "FromIdUser", "ToIdUser" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FriendRequests_FromIdUser_ToIdUser",
                table: "FriendRequests");

            migrationBuilder.RenameColumn(
                name: "ToIdUser",
                table: "FriendRequests",
                newName: "ToUser");

            migrationBuilder.RenameColumn(
                name: "FromIdUser",
                table: "FriendRequests",
                newName: "FromUser");
        }
    }
}
