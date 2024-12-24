using System.Net.WebSockets;

namespace WebSocketExample.Domain;

public sealed class SocketClient(User user, WebSocket socket)
{
    public Guid Id { get; } = Guid.CreateVersion7();

    public User User { get; } = user;
    public WebSocket Socket { get; } = socket;

    public override bool Equals(object? obj)
    {
        if (obj is not SocketClient client) 
            return false;

        return client.Id == Id;
    }

    public override int GetHashCode() => Id.GetHashCode();
}