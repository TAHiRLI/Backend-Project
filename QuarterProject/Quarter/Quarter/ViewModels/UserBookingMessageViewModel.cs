using System.ComponentModel.DataAnnotations;

namespace Quarter.ViewModels
{
    public class UserBookingMessageViewModel
    {
        [MaxLength(30)]
        public string Fullname { get; set; }
        [MaxLength(100)]
        public string Email { get; set; }
        [MaxLength(500)]
        public string Message { get; set; }
        public int HouseId { get; set; }
    }

}
