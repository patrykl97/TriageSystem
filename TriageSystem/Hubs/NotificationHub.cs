using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TriageSystem.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string hospitalID)
        {
            await Clients.All.SendAsync("ReceiveNotification", hospitalID);
        }
    }
}
