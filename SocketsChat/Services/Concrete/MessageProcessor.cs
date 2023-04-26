using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SocketsChat.Models;
using SocketsChat.Services.Abstract;

namespace SocketsChat.Services.Concrete
{
    public class MessageProcessor : IMessageProcessor
    {
        private readonly ISenderService senderService;

        private ConcurrentDictionary<string, string> nickNames = new ConcurrentDictionary<string, string>();

        public MessageProcessor(
            ISenderService senderService)
        {
            this.senderService = senderService;
        }

        public async Task Process(string connectionId, string message)
        {
            var messageObject = JsonConvert.DeserializeObject<Message>(message);

            if (string.IsNullOrEmpty(messageObject?.Body))
                return;

            switch (messageObject.MessageType)
            {
                case MessageTypeEnum.SetName:
                {
                    await this.SetName(connectionId, messageObject.Body);
                    break;
                }
                case MessageTypeEnum.Text:
                {
                    await this.ProcessTextMessage(connectionId, messageObject.Body);
                    break;
                }
            }
        }

        public async Task ProcessDisconnect(string connectionId)
        {
            var name = await this.GetName(connectionId);

            if (!string.IsNullOrEmpty(name))
            {
                var message = new Message
                {
                    MessageType = MessageTypeEnum.Text,
                    SenderName = Constants.SystemSenderName,
                    Body = $"{name} покинул наш чат"
                };

                await this.senderService.SendMessageToAll(message);

                this.RemoveName(connectionId);
            }
        }

        private async Task ProcessTextMessage(string id, string messageBody)
        {
            var name = await this.GetName(id);

            var message = new Message
            {
                MessageType = MessageTypeEnum.Text,
                SenderName = name,
                Body = messageBody,
            };

            await this.senderService.SendMessageToAll(message);
        }

        private async Task SetName(string id, string name)
        {
            try
            {
                if (this.nickNames.Any(x => x.Value.ToLower() == name.ToLower()))
                    throw new Exception("Такое имя уже существует");

                this.nickNames.TryRemove(id, out _);

                this.nickNames.TryAdd(id, name);

                var message = new Message
                {
                    MessageType = MessageTypeEnum.AcceptName,
                    Body = "success"
                };

                await this.senderService.SendMessage(id, message);

                var connectedMessage = new Message
                {
                    MessageType = MessageTypeEnum.Text,
                    SenderName = Constants.SystemSenderName,
                    Body = $"{name} подключился к нашему чату"
                };

                await this.senderService.SendMessageToAll(connectedMessage);
            }
            catch (Exception ex)
            {
                var message = new Message
                {
                    MessageType = MessageTypeEnum.ErrorName,
                    Body = ex.Message
                };

                await this.senderService.SendMessage(id, message);
            }
        }

        private async Task<string> GetName(string id)
        {
            if (this.nickNames.TryGetValue(id, out var name))
            {
                return name;
            }

            return string.Empty;
        }

        private void RemoveName(string id)
        {
            this.nickNames.TryRemove(id, out _);
        }
    }
}
