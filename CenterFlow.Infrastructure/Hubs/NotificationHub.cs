using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CenterFlow.Infrastructure.Hubs
{
    [Authorize]
    public class NotificationHub:Hub
    {
        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier;
            return base.OnConnectedAsync();
        }
    }
}
