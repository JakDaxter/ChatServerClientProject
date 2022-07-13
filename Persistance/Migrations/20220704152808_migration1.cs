using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistance.Migrations
{
    public partial class migration1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    IdChat = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsCreateDefault = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.IdChat);
                });

            migrationBuilder.CreateTable(
                name: "FriendRequests",
                columns: table => new
                {
                    IdFriendRequest = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateTimeWhenRequestWasCreated = table.Column<DateTime>(type: "TEXT", nullable: false),
                    FromUser = table.Column<Guid>(type: "TEXT", nullable: false),
                    ToUser = table.Column<Guid>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FriendRequests", x => x.IdFriendRequest);
                });

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    IdFriends = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdUser1 = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdUser2 = table.Column<Guid>(type: "TEXT", nullable: false),
                    DateTimeWhenRelacionWasCreated = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.IdFriends);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    IdMessage = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExpedationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", maxLength: 280, nullable: false),
                    IdChat = table.Column<Guid>(type: "TEXT", nullable: false),
                    NameOfShipper = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.IdMessage);
                });

            migrationBuilder.CreateTable(
                name: "UserInChats",
                columns: table => new
                {
                    IdUserInChat = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdUser = table.Column<Guid>(type: "TEXT", nullable: false),
                    IdChat = table.Column<Guid>(type: "TEXT", nullable: false),
                    NewMessagesInChat = table.Column<int>(type: "INTEGER", nullable: false),
                    IsMuted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInChats", x => x.IdUserInChat);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    IdUser = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Password = table.Column<string>(type: "TEXT", nullable: false),
                    NrOfFriendRequests = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.IdUser);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IdChat_ExpedationDate",
                table: "Messages",
                columns: new[] { "IdChat", "ExpedationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Name",
                table: "Users",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "FriendRequests");

            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "UserInChats");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
