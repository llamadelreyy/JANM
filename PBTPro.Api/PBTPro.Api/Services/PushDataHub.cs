using Microsoft.AspNetCore.SignalR;

public class PushDataHub : Hub
{
    public static Dictionary<string, string> ConnectedClients = new Dictionary<string, string>();
    
    public override Task OnConnectedAsync()
    {
        string? userId = Context.GetHttpContext().Request.Query["u"];

        if(!string.IsNullOrWhiteSpace(userId))
        {
            var connectionIds = ConnectedClients.Where(kvp => kvp.Value == userId).Select(kvp => kvp.Key).FirstOrDefault();
            if(connectionIds != null)
            {
                ConnectedClients.Remove(connectionIds);
            }
        }

        //var userId = Context.User?.Identity?.Name;
        ConnectedClients[Context.ConnectionId] = userId;
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        ConnectedClients.Remove(Context.ConnectionId);
        return base.OnDisconnectedAsync(exception);
    }

    public static List<KeyValuePair<string, string>> GetConnectedUsers()
    {
        return ConnectedClients.ToList();
    }

    public async Task AddToGroup(string groupName)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task RemoveFromGroup(string groupName)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
    }

    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }

    public async Task SendMessageToUser(string userId, string message)
    {
        var connectionIds = ConnectedClients.Where(kvp => kvp.Value == userId).Select(kvp => kvp.Key);
        foreach (var connectionId in connectionIds)
        {
            await Clients.Client(connectionId).SendAsync("ReceiveMessage", message);
        }
    }

    public async Task SendMessageToGroup(string groupName, string user, string message)
    {
        await Clients.Group(groupName).SendAsync("ReceiveMessage", user, message);
    }
}