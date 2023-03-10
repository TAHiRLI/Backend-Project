using Quarter.Models;

namespace Quarter.ViewModels
{
    public class ProfileViewModel
    {
       public ProfileEditViewModel ProfileEditVm { get; set; } = new ProfileEditViewModel();
        public List<UserBookingMessage> UserBookingMessages { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
    }
}
