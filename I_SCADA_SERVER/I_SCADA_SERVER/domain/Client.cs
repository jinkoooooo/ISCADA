using AltiallInterface.Type.Ethernet.TCP;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I_SCADA_SERVER.domain
{
    class Client
    {
        public AsyncSocketServer.AsyncSocketServerConsumer asyncSocketClient { get; set; }

        public string IP_ADDRESS { get; set; }

        public string USER_ID { get; set; }

        public Client(AsyncSocketServer.AsyncSocketServerConsumer socket, string ip, string id)
        {
            asyncSocketClient = socket;
            IP_ADDRESS = ip;
            USER_ID = id;
        }
    }
}
