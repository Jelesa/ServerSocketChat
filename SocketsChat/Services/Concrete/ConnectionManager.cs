using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;
using SocketsChat.Services.Abstract;

namespace SocketsChat.Services.Concrete
{
    public class ConnectionManager : IConnectionManager
    {
        private ConcurrentDictionary<string, WebSocket> connections = new ConcurrentDictionary<string, WebSocket>();

        public WebSocket GetSocketById(string id)
        {
            if (this.connections.TryGetValue(id, out var value))
                return value;

            throw new Exception($"Подключения с id {id} не существует");
        }

        public ConcurrentDictionary<string, WebSocket> GetAllConnections()
        {
            return this.connections;
        }

        public string GetId(WebSocket socket)
        {
            var result = this.connections.FirstOrDefault(x => x.Value == socket).Key;

            if (result == null)
                throw new Exception("Нет такого socket");

            return result;
        }

        public void AddSocket(WebSocket socket)
        {
            this.connections.TryAdd(GetConnectionId(), socket);
        }

        public async Task RemoveConnectionAsync(string id)
        {
            this.connections.TryRemove(id, out var socket);

            if (socket.State == WebSocketState.Open)
                await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "socket connection closed", CancellationToken.None);
        }

        private static string GetConnectionId()
        {
            return Guid.NewGuid().ToString("N"); //создание id 
        }
    }
}
