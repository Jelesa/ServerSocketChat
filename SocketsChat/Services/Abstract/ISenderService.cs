using System.Net.WebSockets;
using System.Threading.Tasks;
using SocketsChat.Models;

namespace SocketsChat.Services.Abstract
{
    public interface ISenderService
    {
        Task SendMessage(WebSocket socket, Message message);

        Task SendMessage(string id, Message message);

        Task SendMessageToAll(Message message);
    }
}
