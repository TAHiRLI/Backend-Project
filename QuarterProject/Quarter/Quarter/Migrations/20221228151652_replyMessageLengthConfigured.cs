using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Quarter.Migrations
{
    public partial class replyMessageLengthConfigured : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookingRequestReplies_UserBookingMessageId",
                table: "BookingRequestReplies");

            migrationBuilder.AlterColumn<string>(
                name: "ReplyMessage",
                table: "BookingRequestReplies",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequestReplies_UserBookingMessageId",
                table: "BookingRequestReplies",
                column: "UserBookingMessageId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BookingRequestReplies_UserBookingMessageId",
                table: "BookingRequestReplies");

            migrationBuilder.AlterColumn<string>(
                name: "ReplyMessage",
                table: "BookingRequestReplies",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.CreateIndex(
                name: "IX_BookingRequestReplies_UserBookingMessageId",
                table: "BookingRequestReplies",
                column: "UserBookingMessageId");
        }
    }
}
