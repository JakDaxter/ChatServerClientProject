using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public interface IMyRabbitMQProducer
    {
        /*
         * CloseConnection if is open
         */
        public void CloseConnection();
        /*
         * Put Message to a Queue
         * Parameter:
         *  messageContent: The menssage who is set
         *  user: user.Id use to routingKey
         */
        public void NewMessageInConversation(string messageContent, User user);
        /*
         * Put user nr of request to a Queue
         * Parameter:
         *  user: user.Id use to routingKey and user.NrOfFriendRequests use to menssage
         */
        public void FriendRequest(User user);
        /*
         * put "NewChat" in to a Queue
         * Parameter:
         *  user: user.Id use to routingKey
         */
        public void NewChat(User user);
    }
}
