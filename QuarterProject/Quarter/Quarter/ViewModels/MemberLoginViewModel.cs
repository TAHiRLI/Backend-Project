using System.ComponentModel.DataAnnotations;

namespace Quarter.ViewModels
{
    public class MemberLoginViewModel
    {
        [MaxLength(30)]
        public string Username { get; set; }
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }

    }
}
