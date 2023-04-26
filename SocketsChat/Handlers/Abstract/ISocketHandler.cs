using System.Net.WebSockets;
using System.Threading.Tasks;

namespace SocketsChat.Handlers.Abstract
{
    public interface ISocketHandler
    {
        Task OnConnected(WebSocket webSocket);

        Task OnDisconnected(WebSocket webSocket);

        Task OnReceive(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);
    }
}
