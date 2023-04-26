using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SocketsChat.Handlers.Abstract;

namespace SocketsChat.Services.Middleware
{
    public class SocketsMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ISocketHandler handler;

        public SocketsMiddleware(
            RequestDelegate next,
            ISocketHandler handler)
        {
            this.next = next;
            this.handler = handler;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.WebSockets.IsWebSocketRequest)
                return;

            var socket = await context.WebSockets.AcceptWebSocketAsync();

            await this.handler.OnConnected(socket);

            await this.Receive(socket, async (result, buffer) =>
            {
                if (result.MessageType == WebSocketMessageType.Text)
                    await this.handler.OnReceive(socket, result, buffer);

                if (result.MessageType == WebSocketMessageType.Close)
                    await this.handler.OnDisconnected(socket);
            });
        }

        private async Task Receive(WebSocket webSocket, Action<WebSocketReceiveResult, byte[]> messageToHandle)
        {
            var buffer = new byte[1024 * 4];

            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                messageToHandle(result, buffer);
            }
        }
    }
}