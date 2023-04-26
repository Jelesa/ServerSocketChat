using System.Threading.Tasks;

namespace SocketsChat.Services.Abstract
{
    public interface IMessageProcessor
    {
        Task Process(string connectionId, string message);

        Task ProcessDisconnect(string connectionId);
    }
}
