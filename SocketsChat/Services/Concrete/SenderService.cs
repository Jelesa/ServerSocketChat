using System;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SocketsChat.Models;
using SocketsChat.Services.Abstract;

namespace SocketsChat.Services.Concrete
{
    public class SenderService : ISenderService
    {
        private readonly IConnectionManager connectionManager;

        public SenderService(IConnectionManager connectionManager)
        {
            this.connectionManager = connectionManager;
        }

        public async Task SendMessage(WebSocket socket, Message message)
        {
            if (socket.State != WebSocketState.Open)
                return;

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var messageString = JsonConvert.SerializeObject(message, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.Indented
            });

            var messageBytes = Encoding.UTF8.GetBytes(messageString);

            var arraySegment = new ArraySegment<byte>(messageBytes, 0, messageBytes.Length);

            await socket.SendAsync(arraySegment, WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendMessage(string id, Message message)
        {
            await this.SendMessage(this.connectionManager.GetSocketById(id), message);
        }

        public async Task SendMessageToAll(Message message)
        {
            var allClients = this.connectionManager.GetAllConnections().ToArray();

            var taskList = allClients.Select(x => this.SendMessage(x.Value, message)).ToArray();

            await Task.WhenAll(taskList);
        }
    }
}
