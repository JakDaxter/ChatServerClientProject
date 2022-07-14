using log4net;
using Model;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    public class MyRabbitMQProducer : IMyRabbitMQProducer
    {
        private static readonly ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private IModel thisChannel;
        private IConnection thisConnection;
        /*
         * Creating a RabbitMQProducer and setting connection and channel for class
         * Ip:localhost
         * Exchange: "ChatServerClientProject" type: "Direct"
         */
        public MyRabbitMQProducer()
        {
            _log.Info("Creating a RabbitMQ Producer");
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
            };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "ChatServerClientProject", type: ExchangeType.Direct);
            thisConnection = connection;
            thisChannel = channel;
            _log.Info("Created a RabbitMQ Producer");
        }
        /*
         * Close Connection if it is open
         */
        public void CloseConnection()
        {
            _log.Info("Start CloseConnection RabbitMQ");
            if (thisConnection != null)
            {
                thisConnection.Close();
                _log.Info("Connection was colsed");
            }
            _log.Info("The connection is not open");
        }
        /*
         * Put a FriendRequest message in a Queue
         * Parameter:
         *  user: user.Id is used as routingKey and user.NrOfFriendRequests is used to message
         * NotifyFormat:
         *  [FriendRequest]FriendTrquest: {0}
         *  Encoding.UTF8
         */
        public void FriendRequest(User user)
        {
            _log.Info($"Start FriendRequest notify to IdUser: {user.IdUser}");
            string message = $"[FriendRequest]FriendRequest: {user.NrOfFriendRequests}";
            var body = Encoding.UTF8.GetBytes(message);
            this.thisChannel.BasicPublish(exchange: "ChatServerClientProject", routingKey: user.IdUser.ToString(), basicProperties: null, body: body);
            _log.Info("FriendRequest notify completed");
        }
        /*
         * Put Message to a Queue
         * Parameter:
         *  messageContent: The message who is set
         *  user: user.Id is used as routingKey
         * NotifyFormat:
         *  [NewMessageInConversation]{0}
         *  Encoding.UTF8
         */
        public void NewMessageInConversation(string messageContent, User user)
        {
            _log.Info($"Start NewMessageInConversation notify to IdUser {user.IdUser} and Message: {messageContent}");
            string message = $"[NewMessageInConversation]{messageContent}";
            var body = Encoding.UTF8.GetBytes(message);
            this.thisChannel.BasicPublish(exchange: "ChatServerClientProject", routingKey: user.IdUser.ToString(), basicProperties: null, body: body);
            _log.Info("NewMessageInConversation notify completed");
        }
        /*
        * put "NewChat" in to a Queue
        * Parameter:
        *  user: user.Id is used as routingKey
        * NotifyFormat:
        *  [NewChat]
        *  Encoding.UTF8
        */
        public void NewChat(User user)
        {
            _log.Info($"Start NewChat notify from IdUser: {user.IdUser}");
            string message = "[NewChat]";
            var body = Encoding.UTF8.GetBytes(message);
            this.thisChannel.BasicPublish(exchange: "ChatServerClientProject", routingKey: user.IdUser.ToString(), basicProperties: null, body: body);
            _log.Info("NewChat notify completed");
        }
    }
}
