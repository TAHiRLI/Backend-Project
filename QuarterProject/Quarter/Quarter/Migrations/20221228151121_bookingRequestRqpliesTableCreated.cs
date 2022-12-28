using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quarter.Migrations
{
    public partial class bookingRequestRqpliesTableCreated : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BookingRequestReplies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReplyMessage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserBookingMessageId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingRequestReplies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BookingRequestReplies_UserBookingMessages_UserBookingMessageId",
                        column: x => x.UserBookingMessageId,
                        principalTable: "UserBookingMessages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequestReplies_UserBookingMessageId",
                table: "BookingRequestReplies",
                column: "UserBookingMessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingRequestReplies");
        }
    }
}
