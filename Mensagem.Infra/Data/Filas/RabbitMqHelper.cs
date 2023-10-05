using System.Text;
using Mensagem.Domain.Interfaces;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Mensagem.Infra.Data.Filas
{
    public class RabbitMqHelper : IRabbitMQHelper
    {
        public ConnectionFactory GetConnectionFactory()
        {
            var connectionFactory = new ConnectionFactory
            {
                HostName = "127.0.0.1",
                UserName = "guest",
                Password = "guest"
            };
            return connectionFactory;
        }

        public string? RetrieveSingleMessage(string queueName, IConnection connection)
        {
            BasicGetResult data;
            using (var channel = connection.CreateModel())
            {
                data = channel.BasicGet(queueName, true);
            }
            return data != null ? Encoding.UTF8.GetString(data.Body.ToArray()) : null;
        }

        public uint RetrieveMessageCount(string queueName, IConnection connection)
        {
            uint data;
            using (var channel = connection.CreateModel())
            {
                data = channel.MessageCount(queueName);
            }
            return data;
        }

        public IConnection CreateConnection(ConnectionFactory connectionFactory)
        {
            return connectionFactory.CreateConnection();
        }

        public QueueDeclareOk CreateQueue(string queueName, IConnection connection)
        {
            QueueDeclareOk queue;
            using (var channel = connection.CreateModel())
            {
                queue = channel.QueueDeclare(queueName, false, false, false, null);
            }
            return queue;
        }

        public List<string> RetrieveMessageList(string queueName, IConnection connection)
        {
            var messageList = new List<string>();

            using (var channel = connection.CreateModel())
            {
                var messageCount = channel.MessageCount(queueName);
                channel.QueueDeclare(queueName, false, false, false, null);
                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    byte[] body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    messageList.Add(message);

                    int dots = message.Split('.').Length - 1;
                    Thread.Sleep(dots * 1000);

                    // here channel could also be accessed as ((EventingBasicConsumer)sender).Model
                    channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                };
            }
            return messageList;
        }

        public bool WriteMessageOnQueue(string message, string queueName, IConnection connection)
        {
            using (var channel = connection.CreateModel())
            {
                channel.BasicPublish(string.Empty, queueName, null, Encoding.ASCII.GetBytes(message));
            }
            return true;
        }
    }
}