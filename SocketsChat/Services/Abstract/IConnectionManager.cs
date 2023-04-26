using System.Collections.Concurrent;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SocketsChat.Services.Abstract
{
    public interface IConnectionManager
    {
        WebSocket GetSocketById(string id);

        ConcurrentDictionary<string, WebSocket> GetAllConnections();

        string GetId(WebSocket socket);

        void AddSocket(WebSocket socket);

        Task RemoveConnectionAsync(string id);
    }
}
