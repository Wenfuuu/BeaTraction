using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace BeaTraction.Infrastructure.Hubs;

[Authorize]
public class AttractionsHub : Hub
{
    public override async Task OnConnectedAsync()
    {
        await base.OnConnectedAsync();
        Console.WriteLine($"Client connected: {Context.ConnectionId}");
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        await base.OnDisconnectedAsync(exception);
        Console.WriteLine($"Client disconnected: {Context.ConnectionId}");
    }

    public async Task JoinAttractionGroup(string attractionId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"attraction_{attractionId}");
        Console.WriteLine($"Client {Context.ConnectionId} joined attraction group: {attractionId}");
    }

    public async Task LeaveAttractionGroup(string attractionId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"attraction_{attractionId}");
        Console.WriteLine($"Client {Context.ConnectionId} left attraction group: {attractionId}");
    }
}
