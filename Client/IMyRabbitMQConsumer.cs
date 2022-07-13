using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    public interface IMyRabbitMQConsumer
    {
        /*
         * Create a RabbitMQ connection
         * Exchange: ChatServerClientProject
         * RoutingKey: idUser
         * Calling NewEventFromOtherUser from ChatFrom
         * Parameter
         *  idUser: Using to routingKey
         */
        public void CreateConnection(string idUser);
        /*
         * Close a RabbitMQ connection if is open
         */
        public void CloseConnection();
        /*
         * Set a ChatFrom to call when a new data is put in Queue
         * Parameter
         *  chatFrom: set the chantFrom parameter from calls notify
         */
        public void setForm(ChatForm chatForm);
    }
}
