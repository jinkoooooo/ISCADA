using AltiallInterface.Type.Ethernet.TCP;
using I_SCADA_CLIENT.common;
using I_SCADA_CLIENT.domain;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I_SCADA_CLIENT.controller
{
    class SocketController
    {
        public delegate void SocketConnect(string type, string address, int port);
        public event SocketConnect SocketConnectEvent;
        public delegate void SocketDisConnect(string type, string address, int port);
        public event SocketDisConnect SocketDisConnectEvent;
        public delegate void SocketReceived(string body);
        public event SocketReceived SocketReceivedLoginEvent;
        public delegate void SocketReceivedMain(string head, string body);
        public event SocketReceivedMain SocketReceivedMainEvent;

        public SocketController()
        {
            try
            {   // 소켓 및 접속 리스트 초기화.
                CommonData.userSocketConn = null;

                using (StreamReader r = new StreamReader("ServerConn.json"))
                {
                    var json = r.ReadToEnd();
                    SocketConfig item = JsonConvert.DeserializeObject<SocketConfig>(json);
                        CommonData.userSocketConn = item;
                }
            }
            catch (Exception e)
            {
                Logger.All.Error(e.Message);
            }
        }

        public void ServiceStart()
        {
            CommonData.userSocket = new AsyncSocketClient();
            CommonData.userSocket.Address = CommonData.userSocketConn.Address;
            CommonData.userSocket.Port = CommonData.userSocketConn.Port;
            CommonData.userSocket.OnReceived += Socket_OnReceived;
            CommonData.userSocket.OnConnected += (asyncSocketClient) => SocketConnectEvent?.Invoke("connected", asyncSocketClient.Address, asyncSocketClient.Port);
            CommonData.userSocket.OnDisconnected += (asyncSocketClient) => SocketDisConnectEvent?.Invoke("disconnected", asyncSocketClient.Address, asyncSocketClient.Port);
            CommonData.userSocket.Start();
        }

        public void ServiceStop()
        {
            if(CommonData.userSocket != null)
            {
                CommonData.userSocket.Stop();
            }
        }

        private void Socket_OnReceived(AsyncSocketClient asyncSocketClient, byte[] value)
        {
            try
            {
                HeaderHandler.header head = new HeaderHandler.header();
                head.HandleDataReceive(value);
                
                switch (head.SocketNo)
                {
                    case "11":// 로그인 ACK
                        SocketReceivedLoginEvent?.Invoke(head.Body);

                        break;

                    default:
                        SocketReceivedMainEvent?.Invoke(head.SocketNo, head.Body);
                        break;
                    /*case "12":// 로그아웃 ACK
                        SocketReceivedMainEvent?.Invoke(head.SocketNo, head.Body);
                        break;
                    case "13":// 상태 변경 ACK
                        SocketReceivedMainEvent?.Invoke(head.SocketNo, head.Body);
                        break;
                    case "14":// 특정 교수 검색 ACK
                        SocketReceivedMainEvent?.Invoke(head.SocketNo, head.Body);
                        break;
                    case "15":// 메시지 조회 ACK
                        SocketReceivedMainEvent?.Invoke(head.SocketNo, head.Body);
                        break;
                    case "16":// 메시지 답장 ACK
                        SocketReceivedMainEvent?.Invoke(head.SocketNo, head.Body);
                        break;*/

                }
            }
            catch (Exception ex)
            {
                Logger.All.Debug(ex);
            }
        }

        public void SendLogin()
        {
            string SocketNo = "01";

            HeaderHandler.header headerHandler = new HeaderHandler.header();
            if (CommonData.userSocket != null)
                CommonData.userSocket.Send(headerHandler.HeadleDataSend(SocketNo, CommonData.userDomain.UserId + CommonData.userDomain.UserPasswd));
        }

        public void SendStateChange(string state)
        {
            string SocketNo = "03";

            HeaderHandler.header headerHandler = new HeaderHandler.header();
            if (CommonData.userSocket != null)
                CommonData.userSocket.Send(headerHandler.HeadleDataSend(SocketNo, CommonData.userDomain.UserId + state));
        }

        public void SendSearchProf(string name)
        {
            string SocketNo = "04";

            HeaderHandler.header headerHandler = new HeaderHandler.header();
            if (CommonData.userSocket != null)
                CommonData.userSocket.Send(headerHandler.HeadleDataSend(SocketNo, name));
        }

        public void SendSearchMessage()
        {
            string SocketNo = "05";

            HeaderHandler.header headerHandler = new HeaderHandler.header();
            if (CommonData.userSocket != null)
                CommonData.userSocket.Send(headerHandler.HeadleDataSend(SocketNo, CommonData.userDomain.UserId));
        }

        public void SendReMessage(string to_Id, string message)
        {
            string SocketNo = "06";

            HeaderHandler.header headerHandler = new HeaderHandler.header();
            if (CommonData.userSocket != null)
                CommonData.userSocket.Send(headerHandler.HeadleDataSend(SocketNo, to_Id + CommonData.userDomain.UserId + message));
        }

        public void SendSearchAllState()
        {
            string SocketNo = "20";

            HeaderHandler.header headerHandler = new HeaderHandler.header();
            if (CommonData.userSocket != null)
                CommonData.userSocket.Send(headerHandler.HeadleDataSend(SocketNo, CommonData.userDomain.UserId));
        }
    }
}
