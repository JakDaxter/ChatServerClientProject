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
         * Close Connection if it is open
         */
        public void CloseConnection();
        /*
         * Put Message to a Queue
         * Parameter:
         *  messageContent: The message which is set
         *  user: user.Id is used as routingKey
         */
        public void NewMessageInConversation(string messageContent, User user);
        /*
         * Put user nr of request to a Queue
         * Parameter:
         *  user: user.Id is used as routingKey and user.NrOfFriendRequests is used to message
         */
        public void FriendRequest(User user);
        /*
         * put "NewChat" in to a Queue
         * Parameter:
         *  user: user.Id is used as routingKey
         */
        public void NewChat(User user);
    }
}
