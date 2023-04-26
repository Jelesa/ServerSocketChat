using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace SocketsChat.Models
{
    public class Message
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public MessageTypeEnum MessageType { get; set; }

        public string SenderName { get; set; }

        public string Body { get; set; }
    }

    public class Constants
    {
        public const string SystemSenderName = "System";
    }
}
