using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Text;
using WebSocketExample.Domain;

namespace WebApplication1;

public sealed class WebSocketGroupManager(ILogger<WebSocketGroupManager> logger)
{
    private readonly ConcurrentDictionary<string, HashSet<SocketClient>> _groups = [];

    public async Task AddToGroupAsync(string groupName, SocketClient socket, CancellationToken ct)
    {
        var group = _groups.GetOrAdd(groupName, _ => []);

        if (group.Add(socket))
            await MessageGroupAsync(groupName, "User has entered the chat!", ct);
        
        logger.LogInformation("Group ADD ({count})", group.Count);
    }

    public async Task RemoveFromGroup(string groupName, SocketClient socket, CancellationToken ct)
    {
        if (!_groups.TryGetValue(groupName, out var group))
            return;
        
        if (group.Remove(socket))
            await MessageGroupAsync(groupName, "User has left the chat!", ct);
        
        logger.LogInformation("Group REMOVE ({count})", group.Count);
    }

    public async Task BroadcastMessage(string groupName, SocketClient client, string message, CancellationToken ct)
    {
        var payload = $"[{DateTime.UtcNow:hh:mm:ss}] {client.User.Username}: {message}";
        await MessageGroupAsync(groupName, payload, ct);
    }

    private async Task MessageGroupAsync(string groupName, string message, CancellationToken ct)
    {
        if (!_groups.TryGetValue(groupName, out var clients))
            return;
        
        var payloadBytes = new ArraySegment<byte>(Encoding.UTF8.GetBytes(message));
        
        foreach (var client in clients)
        {
            await client.Socket.SendAsync(
                buffer: payloadBytes,
                messageType: WebSocketMessageType.Text,
                endOfMessage: true,
                cancellationToken: ct);
        }
    }
}