using Microsoft.AspNetCore.SignalR;

namespace PresentationApp.Hubs;

public class TestHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await Clients.All.SendAsync("UserConnected", Context.ConnectionId);
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await Clients.All.SendAsync("UserDisconnected", Context.ConnectionId);
        await base.OnDisconnectedAsync(exception);
    }

    public async Task SendTestMessage(string message)
    {
        await Clients.All.SendAsync("ReceiveTestMessage", Context.ConnectionId, message);
    }
}