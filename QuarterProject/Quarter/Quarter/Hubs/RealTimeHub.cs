using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Quarter.Models;

namespace Quarter.Hubs
{
    public class RealTimeHub:Hub
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IHttpContextAccessor _httpAccessor;

        public RealTimeHub(UserManager<AppUser> userManager,IHttpContextAccessor httpAccessor)
        {
            this._userManager = userManager;
            this._httpAccessor = httpAccessor;
        }
        public override async Task OnConnectedAsync()
        {
            if (_httpAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(_httpAccessor.HttpContext.User.Identity.Name);
                if (user != null)
                {
                    user.ConnectionId = Context.ConnectionId;
                    user.LastConnectedAt = DateTime.UtcNow.AddHours(4);
                    var result = await _userManager.UpdateAsync(user);
                    await Clients.All.SendAsync("setAsOnline" , user.Id);
                }
            }
            base.OnConnectedAsync();
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            if (_httpAccessor.HttpContext.User.Identity.IsAuthenticated)
            {
                var user = await _userManager.FindByNameAsync(_httpAccessor.HttpContext.User.Identity.Name);
                if (user != null)
                {
                    user.ConnectionId = null;
                    user.LastConnectedAt = DateTime.UtcNow.AddHours(4);
                    var result = await _userManager.UpdateAsync(user);
                    await Clients.All.SendAsync("setAsOffline", user.Id );
                }
            }


             base.OnDisconnectedAsync(exception);
        }
    }
}
