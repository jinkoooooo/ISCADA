using AltiallInterface.Type.Ethernet.TCP;
using I_SCADA_SERVER.common;
using I_SCADA_SERVER.controller;
using I_SCADA_SERVER.dao;
using I_SCADA_SERVER.domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace I_SCADA_SERVER
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {

        ViewOrderListContext _context = new ViewOrderListContext();

        public MainWindow()
        {
            InitializeComponent();

            CommonData.serverDao = new ServerDao(ConfigurationManager.AppSettings.Get("db_connection_scada"));

            CommonData.Socket_Server = new AsyncSocketServer(5001);

            this.DataContext = _context;
        }

        private void BtnProgramClose(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BtnServerStart(object sender, RoutedEventArgs e)
        {
            try
            {
                CommonData.Socket_Server.OnListening += Socket_Server_OnListening;
                CommonData.Socket_Server.OnReceived += Socket_Server_OnReceived;
                CommonData.Socket_Server.OnConnected += Socket_Server_OnConnected;
                CommonData.Socket_Server.OnDisconnected += Socket_Server_OnDisconnected;
                CommonData.Socket_Server.Start();

                TbxLogger("I_SCADA_SERVER.Start...");

                ServerFlagController(false);
            }
            catch (Exception ex)
            {
                Logger.All.Debug(ex.ToString());
            }
        }

        private void BtnServerStop(object sender, RoutedEventArgs e)
        {
            CommonData.Socket_Server.Stop();
            TbxLogger("I_SCADA_SERVER.Stop!!!");

            ServerFlagController(true);
        }

        private void ServerFlagController(bool flag)
        {
            try
            {
                _context.ServerStartFlag = flag;
                _context.ServerStopFlag = !flag;
            }
            catch (Exception ex)
            {
                Logger.All.Debug(ex.ToString());
            }
        }

        private void TbxLogger(string text)
        {
            _context.LoggerStr += $"[{DateTime.Now}] {text} \n";
        }

        private void Socket_Server_OnDisconnected(AsyncSocketServer asyncSocketServer, AsyncSocketServer.AsyncSocketServerConsumer asyncSocketClient)
        {
            List<Client> tempList = new List<Client>();
            foreach (Client item in _context.Client_List)
            {
                if (item.IP_ADDRESS == asyncSocketClient.Name)
                {
                    continue;
                }
                tempList.Add(item);
            } 

            _context.Client_List = tempList;

            TbxLogger($"{asyncSocketClient.Name} OnDisConnected");
        }

        private void Socket_Server_OnConnected(AsyncSocketServer asyncSocketServer, AsyncSocketServer.AsyncSocketServerConsumer asyncSocketClient)
        {
            TbxLogger($"{asyncSocketClient.Name} OnConnected");
        }

        private void Socket_Server_OnReceived(AsyncSocketServer asyncSocketServer, AsyncSocketServer.AsyncSocketServerConsumer asyncSocketClient, byte[] value)
        {
            try
            {
                string a = "";
                foreach (byte i in value)
                    a += i.ToString();
                TbxLogger($"Socket Recv(10진수) [{a}]");
                TbxLogger($"Socket Recv(ASCII) [STX{Encoding.Default.GetString(value)}ETX]");

                HeaderHandler.header head = new HeaderHandler.header();
                head.HandleDataReceive(value);
                
                switch (head.SocketNo)
                {
                    case "20":
                        List<AllState> state_List = CommonData.serverDao.GetStateAll();

                        for (int i = 0; i < state_List.Count; i++)
                        {
                            asyncSocketClient.Send(head.HeadleDataSend("21", state_List[i].NOW_STATE + state_List[i].PRO_ADDRESS));
                            Thread.Sleep(50);
                        }

                        break;
                    case "01":// 로그인
                        TbxLogger($"{asyncSocketClient.Name} : Request Login");

                        string login_id = head.Body.Substring(0, 9);
                        string login_pw = head.Body.Substring(9);

                        List<MD_USER> User_List = CommonData.serverDao.SelectMDUser();

                        MD_USER Select_MD = User_List.Where(i => i.USER_ID == login_id).FirstOrDefault();

                        if(Select_MD == null)//아이디 없음.
                        {
                            asyncSocketClient.Send(head.HeadleDataSend("11","1"));
                            TbxLogger($"{asyncSocketClient.Name} : Failed Login - Not exist ID");

                            a = "";
                            foreach (byte i in head.HeadleDataSend("11", "0"))
                                a += i.ToString();
                            TbxLogger($"Socket Send(10진수) [{a}]");
                            TbxLogger($"Socket Send(ASCII) [STX{Encoding.Default.GetString(head.HeadleDataSend("11", "1"))}ETX]");
                        }
                        else
                        {
                            if(Select_MD.USER_PW != login_pw)//비밀번호 틀림.
                            {
                                asyncSocketClient.Send(head.HeadleDataSend("11", "2"));
                                TbxLogger($"{asyncSocketClient.Name} : Failed Login - Not match PW");

                                a = "";
                                foreach (byte i in head.HeadleDataSend("11", "0"))
                                    a += i.ToString();
                                TbxLogger($"Socket Send(10진수) [{a}]");
                                TbxLogger($"Socket Send(ASCII) [STX{Encoding.Default.GetString(head.HeadleDataSend("11", "2"))}ETX]");
                            }
                            else//로그인 성공
                            {
                                asyncSocketClient.Send(head.HeadleDataSend("11", "0"));
                                TbxLogger($"{asyncSocketClient.Name} : Success Login");

                                a = "";
                                foreach (byte i in head.HeadleDataSend("11", "0"))
                                    a += i.ToString();
                                TbxLogger($"Socket Send(10진수) [{a}]");
                                TbxLogger($"Socket Send(ASCII) [STX{Encoding.Default.GetString(head.HeadleDataSend("11", "0"))}ETX]");

                                List<Client> tempList = new List<Client>();
                                foreach (Client item in _context.Client_List)
                                    tempList.Add(item);
                                tempList.Add(new Client(asyncSocketClient, asyncSocketClient.Name, head.Body.Substring(0, 9)));

                                _context.Client_List = tempList;
                            }
                        }

                        break;
                    case "03":// 상태 변경
                        try
                        {
                            TB_PRO_STATE param = new TB_PRO_STATE();
                            param.USER_ID = head.Body.Substring(0, 9);
                            param.STATE = head.Body.Substring(9);

                            CommonData.serverDao.UpdateState(param);

                            asyncSocketClient.Send(head.HeadleDataSend("13", param.STATE));
                        }
                        catch (Exception e)
                        {
                            asyncSocketClient.Send(head.HeadleDataSend("00", e.ToString()));
                        }
                        break;
                    case "04":// 특정 교수 검색
                        try
                        {
                            SearchState param = new SearchState();
                            param.PROF_NAME = head.Body;

                            param = CommonData.serverDao.GetState(param);

                            if(param != null)
                            {
                                param.PROF_NAME = head.Body;
                                asyncSocketClient.Send(head.HeadleDataSend("14", param.PROF_ID + param.STATE + param.PROF_NAME));
                            }
                            else
                            {
                                asyncSocketClient.Send(head.HeadleDataSend("00", head.Body + " 교수님은 존재하지 않습니다."));
                            }
                            
                        }
                        catch (Exception e)
                        {
                            asyncSocketClient.Send(head.HeadleDataSend("00", e.ToString()));
                        }
                        break;
                    case "05":// 메시지 조회
                        try
                        {
                            MD_USER param = new MD_USER();
                            param.USER_ID = head.Body.Substring(0, 9);

                            List<Message> searchMessage = new List<Message>();
                            searchMessage = CommonData.serverDao.GetMessage(param);

                            List<MD_USER> user_data_List = CommonData.serverDao.SelectMDUser();
                            

                            for (int i = 0; i < searchMessage.Count; i++)
                            {
                                
                                MD_USER search_MD = user_data_List.Where(j => j.USER_ID == searchMessage[i].FROM_ID).FirstOrDefault();

                                if(search_MD != null)
                                {
                                    asyncSocketClient.Send(head.HeadleDataSend("15", searchMessage[i].FROM_ID + search_MD.USER_NAME + searchMessage[i].MESSAGE));
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            asyncSocketClient.Send(head.HeadleDataSend("00", e.ToString()));
                        }
                        break;
                    case "06":// 메시지 답장
                        try
                        {
                            Message param = new Message();
                            param.TO_ID = head.Body.Substring(0, 9);
                            param.FROM_ID = head.Body.Substring(9, 9);
                            param.MESSAGE = head.Body.Substring(18);

                            List<MD_USER> user_data_List = CommonData.serverDao.SelectMDUser();

                            MD_USER search_MD = user_data_List.Where(j => j.USER_ID == param.TO_ID).FirstOrDefault();

                            if(search_MD != null)//받는 아이디 존재 확인.
                            {
                                CommonData.serverDao.InsertMessage(param);
                                asyncSocketClient.Send(head.HeadleDataSend("16", "0"));//성공
                            }
                            else{
                                asyncSocketClient.Send(head.HeadleDataSend("00", "존재하지 않는 USER ID 입니다."));
                            }
                        }
                        catch (Exception e)
                        {
                            asyncSocketClient.Send(head.HeadleDataSend("00", e.ToString()));
                        }
                        break;
                    case "02":// 로그아웃

                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                Logger.All.Debug(ex);
            }
        }

        private void Socket_Server_OnListening(AsyncSocketServer asyncSocketServer)
        {
            TbxLogger("Client Listening......");
        }

        private void TbxFocusBottom(object sender, TextChangedEventArgs e)
        {
            tbxLog.CaretIndex = tbxLog.Text.Length;
            tbxLog.ScrollToEnd();
        }
    }

    class ViewOrderListContext : INotifyPropertiesBase
    {
        #region UI binding Context
        private List<Client> client_list = new List<Client>();
        public List<Client> Client_List { get => client_list; set { client_list = value; NotifyPropertyChanged(); } }

        private bool serverStartFlag = true;
        public bool ServerStartFlag { get => serverStartFlag; set { serverStartFlag = value; NotifyPropertyChanged(); } }

        private bool serverStopFlag = false;
        public bool ServerStopFlag { get => serverStopFlag; set { serverStopFlag = value; NotifyPropertyChanged(); } }

        private string loggerStr = "";
        public string LoggerStr { get => loggerStr; set { loggerStr = value; NotifyPropertyChanged(); } }

        #endregion
    }
}
