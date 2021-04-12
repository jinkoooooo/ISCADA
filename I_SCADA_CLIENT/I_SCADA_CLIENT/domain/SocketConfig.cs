using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I_SCADA_CLIENT.domain
{
    class SocketConfig
    {
        public string ID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public int Port { get; set; }

        public SocketConfig(string id, string name, string type, string address, int port)
        {
            ID = id;
            Name = name;
            Type = type;
            Address = address;
            Port = port;
        }
    }
}
