using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using WebApplication1;
using WebSocketExample.Domain;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<WebSocketClientHandler>();
builder.Services.AddSingleton<WebSocketGroupManager>();

var app = builder.Build();

app.UseWebSockets();

app.Map("/ws", async (
    HttpContext context,
    CancellationToken ct,
    [FromServices] WebSocketClientHandler socketHandler) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        var buffer = new byte[1024 * 4];
        
        // The first message by a client is assumed to be a 'handshake' where
        // the client sends a User JSON object containing their information (such as Username)
        var initialPayload = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), ct);
        if (initialPayload.MessageType is WebSocketMessageType.Close)
        {
            await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing pre-maturely", ct);
            return;
        }
        if (initialPayload.MessageType is not WebSocketMessageType.Text)
        {
            context.Response.StatusCode = 400;
            return;
        }

        try
        {
            var json = Encoding.UTF8.GetString(buffer, 0, initialPayload.Count);
            var user = JsonSerializer.Deserialize<User>(json, JsonSerializerOptions.Web);

            if (user is null)
            {
                context.Response.StatusCode = 400;
                return;
            }
            
            var client = new SocketClient(user, webSocket);
            
            // After successfully creating a SocketClient, we can finally invoke the WebSocketClientHandler.
            // From here the server and client will be able to communicate freely.
            await socketHandler.HandleSocket(client, ct);
        }
        catch (Exception)
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();