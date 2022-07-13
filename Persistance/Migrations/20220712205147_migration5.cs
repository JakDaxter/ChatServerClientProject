using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    public partial class migration5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ToIdUser",
                table: "FriendRequests",
                newName: "IdToUser");

            migrationBuilder.RenameColumn(
                name: "FromIdUser",
                table: "FriendRequests",
                newName: "IdFromUser");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequests_FromIdUser_ToIdUser",
                table: "FriendRequests",
                newName: "IX_FriendRequests_IdFromUser_IdToUser");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdToUser",
                table: "FriendRequests",
                newName: "ToIdUser");

            migrationBuilder.RenameColumn(
                name: "IdFromUser",
                table: "FriendRequests",
                newName: "FromIdUser");

            migrationBuilder.RenameIndex(
                name: "IX_FriendRequests_IdFromUser_IdToUser",
                table: "FriendRequests",
                newName: "IX_FriendRequests_FromIdUser_ToIdUser");
        }
    }
}
