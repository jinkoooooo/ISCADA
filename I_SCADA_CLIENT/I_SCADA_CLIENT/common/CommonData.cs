using AltiallInterface.Type.Ethernet.TCP;
using I_SCADA_CLIENT.controller;
using I_SCADA_CLIENT.domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace I_SCADA_CLIENT.common {
    class CommonData {

        public static UserDomain userDomain = new UserDomain();

        public static SocketConfig userSocketConn;

        public static AsyncSocketClient userSocket;

        public static SocketController socketController;

        public static string LocalIPAddress() // 중복실행 방지시 사용.
        {
            IPHostEntry host;
            string localIP = "";
            host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList) {
                if (ip.AddressFamily == AddressFamily.InterNetwork) {
                    localIP = ip.ToString();
                    return localIP;
                }
            }
            return "127.0.0.1";
        }
    }
}
