using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public class MyRabbitMQConsumer : IMyRabbitMQConsumer
    {
        private static readonly NLog.Logger _log = NLog.LogManager.GetCurrentClassLogger();
        private IConnection _connection;
        private ChatForm _form = null;
        public void CloseConnection()
        {
            _log.Info("Starting Closing Connection");
            if (_connection != null && _connection.IsOpen)
            {
                _connection.Close();
                _log.Info("Closing Corectly\n");
                return;
            }
            _log.Info("connection does not exist or is closed\n");

        }
        public void setForm(ChatForm chatForm)
        {
            _form = chatForm;
            _log.Info("set ChatFrom for LoginForm\n");
        }
        public void CreateConnection(string idUser)
        {
            _log.Info($"Creating a RabbitMQ Consumer connection from {idUser}");
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            {
                channel.ExchangeDeclare(exchange: "ChatServerClientProject", type: ExchangeType.Direct);

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queue: queueName,
                                  exchange: "ChatServerClientProject",
                                  routingKey: idUser);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    try
                    {
                        _form.NewEventFromOtherUserAsync(message);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message);
                    }
                };
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);
                _connection = connection;
            }
            _log.Info("connection was created corectly\n");
        }
    }
}
