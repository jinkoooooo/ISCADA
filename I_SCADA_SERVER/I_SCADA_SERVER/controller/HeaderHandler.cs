using AltiallInterface.Type.Ethernet.TCP;
using I_SCADA_SERVER.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace I_SCADA_SERVER.controller
{
    public class HeaderHandler
    {
        public class header
        {
            public const char STX = (char)0x02;
            public const char ETX = (char)0x03;

            private static object lockObject = new object();

            public string SocketNo { get; set; }

            public string Body { get; set; }

            public void HandleDataReceive(byte[] value)
            {
                lock(lockObject)
                {
                    try
                    {
                        List<byte> recvBuffer = new List<byte>(); 
                        List<byte> compData = null;
                        bool existStx = false;
                        recvBuffer.AddRange(value);
                        for (int i = 0; i < recvBuffer.Count; i++)
                        {
                            if (recvBuffer[i] == (byte)STX)
                            {
                                compData = new List<byte>();
                                existStx = true;
                                continue;
                            }
                            if (recvBuffer[i] == (byte)ETX)
                            {
                                if (existStx)
                                {
                                    ReceiveHead(compData.ToArray());
                                    existStx = false;
                                }
                                else
                                {
                                    compData = new List<byte>();
                                }
                                recvBuffer.RemoveRange(0, i);
                                i = 0;
                                continue;
                            }
                            compData.Add(recvBuffer[i]);
                        }
                    }
                    catch(Exception e)
                    {
                        Logger.All.Debug(e.Message);
                    }
                }
            }

            public byte[] HeadleDataSend(string header, string data)
            {
                try
                {
                    return Encoding.Default.GetBytes(STX + header + data + ETX);
                }
                catch (Exception ex)
                {
                    Logger.All.Debug(ex);
                    return null;
                }
            }

            private void ReceiveHead(byte[] value)
            {
                SocketNo = Encoding.Default.GetString(value).Substring(0, 2);

                Body = Encoding.Default.GetString(value).Substring(2);
            }
        }
    }
}
