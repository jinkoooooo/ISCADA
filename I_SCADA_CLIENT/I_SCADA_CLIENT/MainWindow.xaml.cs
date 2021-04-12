using I_SCADA_CLIENT.common;
using I_SCADA_CLIENT.controller;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using System.Windows.Threading;

namespace I_SCADA_CLIENT
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        StateContext _context = new StateContext();
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = _context;
            
            //CommonData.socketController = new SocketController();
            CommonData.socketController.SocketConnectEvent += SocketController_SocketConnectEvent;
            CommonData.socketController.SocketReceivedMainEvent += SocketController_SocketReceivedMainEvent;
            //CommonData.socketController.ServiceStart();

            _context.IsBrushFill = State_4.Fill;
            btn4.IsEnabled = false;
        }

        private void SocketController_SocketConnectEvent(string type, string address, int port)
        {
            CommonData.socketController.SendLogin();
        }

        private void SocketController_SocketReceivedMainEvent(string head, string body)
        {
            switch (head)
            {
                case "12":// 로그아웃 ACK

                    break;
                case "13":// 상태 변경 ACK
                    StateChanged(body);
                    break;
                case "14":// 특정 교수 검색 ACK

                    break;
                case "15":// 메시지 조회 ACK

                    break;
                case "16":// 메시지 답장 ACK

                    break;
                case "00":// 서버 ERROR
                    MessageBox.Show(body);
                    break;
            }
        }

        private void StateChanged(string body)
        {
            
            switch (body)
            {
                case "0":
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _context.IsBrushFill = State_3.Fill;
                        btn1.IsEnabled = false;
                        btn2.IsEnabled = true;
                        btn3.IsEnabled = true;
                        btn4.IsEnabled = true;
                    }));
                    break;
                case "1":
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _context.IsBrushFill = State_1.Fill;
                        btn1.IsEnabled = true;
                        btn2.IsEnabled = false;
                        btn3.IsEnabled = true;
                        btn4.IsEnabled = true;
                    }));
                    break;
                case "2":
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _context.IsBrushFill = State_2.Fill;
                        btn1.IsEnabled = true;
                        btn2.IsEnabled = true;
                        btn3.IsEnabled = false;
                        btn4.IsEnabled = true;
                    }));
                    break;
                case "3":
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        _context.IsBrushFill = State_4.Fill;
                        btn1.IsEnabled = true;
                        btn2.IsEnabled = true;
                        btn3.IsEnabled = true;
                        btn4.IsEnabled = false;
                    }));
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CommonData.userSocket != null)
            {
                CommonData.userSocket.Stop();
            }
            this.Close();
            //mainWindow.ShowDialog();
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BtnState_Change(object sender, RoutedEventArgs e)
        {
            if (sender == btn1)
                CommonData.socketController.SendStateChange("0");
            else if (sender == btn2)
                CommonData.socketController.SendStateChange("1");
            else if (sender == btn3)
                CommonData.socketController.SendStateChange("2");
            else if (sender == btn4)
                CommonData.socketController.SendStateChange("3");
        }

        private  void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SearchProf searchProf = new SearchProf();

            searchProf.Show();

            CommonData.socketController.SocketConnectEvent -= SocketController_SocketConnectEvent;
            CommonData.socketController.SocketReceivedMainEvent -= SocketController_SocketReceivedMainEvent;
            this.Close();
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }

        private void Btn_Message(object sender, RoutedEventArgs e)
        {
            CommonData.socketController.SocketConnectEvent -= SocketController_SocketConnectEvent;
            CommonData.socketController.SocketReceivedMainEvent -= SocketController_SocketReceivedMainEvent;

            MessageWindow messageWindow = new MessageWindow();
            messageWindow.Show();
            this.Close();
        }

        private void Btn_LogOut(object sender, RoutedEventArgs e)
        {
            if (CommonData.userSocket != null)
            {
                CommonData.userSocket.Stop();
            }

            LoginWindow loginWindow = new LoginWindow();
            loginWindow.Show();
            this.Close();
        }

        private void Btn_Scada(object sender, RoutedEventArgs e)
        {
            CommonData.socketController.SocketConnectEvent -= SocketController_SocketConnectEvent;
            CommonData.socketController.SocketReceivedMainEvent -= SocketController_SocketReceivedMainEvent;

            ScadaWindow scadaWindow = new ScadaWindow();
            scadaWindow.Show();
            this.Close();
        }
    }

    class StateContext : INotifyPropertiesBase
    {
        public Brush IsBrushFill { get => isBrushFill; set { isBrushFill = value; NotifyPropertyChanged(); } }
        private Brush isBrushFill = null;

        public bool IsBackgroundWork { get => isBackgroundWork; set { isBackgroundWork = value; NotifyPropertyChanged(); } }
        public string BackgroundWorkMessage { get => backgroundWorkMessage; set { backgroundWorkMessage = value; NotifyPropertyChanged(); } }

        public int IsBackgroundWorkProgress { get => isBackgroundWorkProgress; set { isBackgroundWorkProgress = value; NotifyPropertyChanged(); } }

        public bool IsIndeterminate { get => isIndeterminate; set { isIndeterminate = value; NotifyPropertyChanged(); } }


        private int isBackgroundWorkProgress = 0;
        private bool isBackgroundWork = false;
        private bool isIndeterminate = false;
        private string backgroundWorkMessage = "";
    }
}
