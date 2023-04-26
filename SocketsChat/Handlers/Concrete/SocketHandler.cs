using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using SocketsChat.Handlers.Abstract;
using SocketsChat.Services.Abstract;

namespace SocketsChat.Handlers.Concrete
{
    public class SocketHandler : ISocketHandler
    {
        private readonly IConnectionManager connectionManager;
        private readonly IMessageProcessor messageProcessor;

        public SocketHandler(
            IConnectionManager connectionManager,
            IMessageProcessor messageProcessor)
        {
            this.connectionManager = connectionManager;
            this.messageProcessor = messageProcessor;
        }

        public async Task OnConnected(WebSocket webSocket)
        {
            await Task.Run(() => this.connectionManager.AddSocket(webSocket));
        }

        public async Task OnDisconnected(WebSocket webSocket)
        {
            var socketId = this.connectionManager.GetId(webSocket);

            await Task.Run(() => this.connectionManager.RemoveConnectionAsync(socketId));

            await this.messageProcessor.ProcessDisconnect(socketId);
        }

        public async Task OnReceive(WebSocket webSocket, WebSocketReceiveResult result, byte[] buffer)
        {
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            await this.messageProcessor.Process(this.connectionManager.GetId(webSocket), message);
        }
    }
}
