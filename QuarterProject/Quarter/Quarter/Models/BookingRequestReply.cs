using System.ComponentModel.DataAnnotations;

namespace Quarter.Models
{
    public class BookingRequestReply
    {
        public int Id { get; set; }
        [MaxLength(150)]
        public string ReplyMessage { get; set; }
        public int UserBookingMessageId { get; set; }
        public UserBookingMessage? UserBookingMessage { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow.AddHours(4);
    }
}
