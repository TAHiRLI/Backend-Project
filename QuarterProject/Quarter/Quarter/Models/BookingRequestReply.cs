using System.ComponentModel.DataAnnotations;

namespace Quarter.Models
{
    public class BookingRequestReply:BaseEntity
    {
        [MaxLength(150)]
        public string ReplyMessage { get; set; }
        public int UserBookingMessageId { get; set; }
        public UserBookingMessage? UserBookingMessage { get; set; }
    }
}
