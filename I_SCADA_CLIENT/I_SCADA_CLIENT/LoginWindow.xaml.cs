using I_SCADA_CLIENT.common;
using I_SCADA_CLIENT.controller;
using I_SCADA_CLIENT.domain;
using System;
using System.Collections.Generic;
using System.Linq;
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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace I_SCADA_CLIENT
{
    /// <summary>
    /// LoginWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LoginWindow : Window
    {


        LoginContext _context = new LoginContext();
        
        public LoginWindow()
        {
            InitializeComponent();

            this.DataContext = _context;

            CommonData.userDomain = new UserDomain();
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void Txt_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Enter)
            {
                //OKOKOK();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private async void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                if (txtId.Text.Length != 9)
                {
                    _context.LoggerStr = "ID(학번)을 똑바로 입력하세요.!!!(9자리)";
                }
                else
                {
                    await Task.Run(() => {
                        IndicatorView(true, "서버 연결 시도중...");
                        Thread.Sleep(3000);
                    });

                    CommonData.socketController = new SocketController();
                    CommonData.socketController.SocketConnectEvent += SocketController_SocketConnectEvent;
                    CommonData.socketController.SocketDisConnectEvent += SocketController_SocketDisConnectEvent;
                    CommonData.socketController.SocketReceivedLoginEvent += SocketController_SocketReceivedLoginEvent;
                    CommonData.socketController.ServiceStart();
                }
            }
            catch (Exception ex)
            {
                Logger.All.Debug(ex);
                await Task.Run(() => {
                    IndicatorView(true, "로그인 실패 - Server Connecting ERROR");

                    Thread.Sleep(3000);
                    _context.LoggerStr = "로그인 실패 - Server Connecting ERROR";
                });
                IndicatorView(false);
            }

        }

        private void SocketController_SocketReceivedLoginEvent(string body)
        {
            try
            {
                switch (body)
                {
                    case "0":
                        Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                        {
                            CommonData.socketController.SocketConnectEvent -= SocketController_SocketConnectEvent;
                            CommonData.socketController.SocketDisConnectEvent -= SocketController_SocketDisConnectEvent;
                            CommonData.socketController.SocketReceivedLoginEvent -= SocketController_SocketReceivedLoginEvent;
                            IndicatorView(false);
                            MainWindow main = new MainWindow();
                            main.Show();
                            this.Close();
                        }));
                        break;
                    case "1":
                        IndicatorView(true, "로그인 실패 - Not exist User_ID!!");
                        Thread.Sleep(3000);
                        IndicatorView(false);
                        _context.LoggerStr = "로그인 실패 - Not exist User_ID!!";
                        CommonData.socketController.ServiceStop();
                        break;
                    case "2":
                        IndicatorView(true, "로그인 실패 - Not match User_Pw!!");
                        Thread.Sleep(3000);
                        IndicatorView(false);
                        _context.LoggerStr = "로그인 실패 - Not match User_Pw!!";
                        CommonData.socketController.ServiceStop();
                        break;
                    default:
                        CommonData.socketController.ServiceStop();
                        break;
                }
            }
            catch (Exception e)
            {
                Logger.All.Debug(e.ToString());
            }

        }

        private void SocketController_SocketDisConnectEvent(string type, string address, int port)
        {
            
        }

        private async void SocketController_SocketConnectEvent(string type, string address, int port)
        {
            try
            {
                await Task.Run(() => {
                    IndicatorView(true, "서버 연결 성공. 유저정보확인중입니다.");
                });
                Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                {
                    CommonData.userDomain.UserId = txtId.Text;
                    CommonData.userDomain.UserPasswd = txtPasswd.Password.ToString();

                    CommonData.socketController.SendLogin();
                }));
            }
            catch (Exception e)
            {
                await Task.Run(() => {
                    IndicatorView(true, "서버 연결 에러. 재시도 하세요.");
                    Thread.Sleep(3000);
                    IndicatorView(false);
                });
                Logger.All.Debug(e.ToString());
            }
        }

        private void IndicatorView(bool isWork, string message = "작업중입니다.", bool isIndeterminate = true)
        {
            _context.IsIndeterminate = isIndeterminate;
            _context.IsBackgroundWork = isWork;
            _context.BackgroundWorkMessage = message;
        }
        
    }

    class LoginContext : INotifyPropertiesBase
    {

        public bool IsBackgroundWork { get => isBackgroundWork; set { isBackgroundWork = value; NotifyPropertyChanged(); } }
        public string BackgroundWorkMessage { get => backgroundWorkMessage; set { backgroundWorkMessage = value; NotifyPropertyChanged(); } }

        public int IsBackgroundWorkProgress { get => isBackgroundWorkProgress; set { isBackgroundWorkProgress = value; NotifyPropertyChanged(); } }

        public bool IsIndeterminate { get => isIndeterminate; set { isIndeterminate = value; NotifyPropertyChanged(); } }


        private int isBackgroundWorkProgress = 0;
        private bool isBackgroundWork = false;
        private bool isIndeterminate = false;
        private string backgroundWorkMessage = "";

        private string loggerStr = "";
        public string LoggerStr { get => loggerStr; set { loggerStr = value; NotifyPropertyChanged(); } }

    }
}
