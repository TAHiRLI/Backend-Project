using Microsoft.AspNetCore.Authentication;
using System.ComponentModel.DataAnnotations;

namespace Quarter.ViewModels
{
    public class MemberLoginViewModel
    {
        public string Username { get; set; }
        [DataType(DataType.Password)]
        [MinLength(8)]
        public string Password { get; set; }
        public IList<AuthenticationScheme>? ExternalLogins { get; set; }

    }
}
