using System.Net.WebSockets;
using System.Text;
using WebSocketExample.Domain;

namespace WebApplication1;

public sealed class WebSocketClientHandler(WebSocketGroupManager groupManager)
{
    public async Task HandleSocket(SocketClient client, CancellationToken ct)
    {
        var buffer = new byte[1024 * 4];

        try
        {
            await groupManager.AddToGroupAsync("Test", client, ct);
            WebSocketReceiveResult receiveResult;
            
            do
            {
                receiveResult = await client.Socket.ReceiveAsync(
                    buffer: new ArraySegment<byte>(buffer),
                    cancellationToken: ct);

                var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                await groupManager.BroadcastMessage("Test", client, message, ct);

            } while (!receiveResult.CloseStatus.HasValue);
        }
        finally
        {
            await groupManager.RemoveFromGroup("Test", client, ct);
            await client.Socket.CloseAsync(
                closeStatus: WebSocketCloseStatus.NormalClosure,
                statusDescription: "Disconnected",
                ct);
        }
    }
}