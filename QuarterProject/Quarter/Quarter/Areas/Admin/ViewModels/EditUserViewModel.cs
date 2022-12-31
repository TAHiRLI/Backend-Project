namespace Quarter.Areas.Admin.ViewModels
{
    public class EditUserViewModel
    {
        public string UserId { get; set; }  
        public List<string> RoleNames { get; set; } = new List<string>();
    }
}
