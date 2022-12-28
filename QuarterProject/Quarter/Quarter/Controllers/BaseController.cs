using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Quarter.Controllers
{
    public class BaseController : Controller
    {
        protected string UserId => User.FindFirstValue(ClaimTypes.NameIdentifier);
    }
}
