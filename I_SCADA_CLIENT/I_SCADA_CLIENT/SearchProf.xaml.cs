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
using System.Windows.Shapes;
using System.Windows.Threading;

namespace I_SCADA_CLIENT
{
    /// <summary>
    /// SearchProf.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class SearchProf : Window
    {
        StateContext2 _context = new StateContext2();

        public SearchProf()
        {
            InitializeComponent();
            
            CommonData.socketController.SocketConnectEvent += SocketController_SocketConnectEvent;
            CommonData.socketController.SocketReceivedMainEvent += SocketController_SocketReceivedMainEvent;

            this.DataContext = _context;
        }

        private void SocketController_SocketConnectEvent(string type, string address, int port)
        {
            CommonData.socketController.SendLogin();
            IndicatorView();
        }

        private void SocketController_SocketReceivedMainEvent(string head, string body)
        {
            switch (head)
            {
                case "14":// 특정 교수 검색 ACK
                    string id = body.Substring(0, 9);
                    string state = body.Substring(9, 1);
                    string name = body.Substring(10);

                    switch (state)
                    {
                        case "0":
                            state = "상담 가능";
                            break;
                        case "1":
                            state = "상담 중";
                            break;
                        case "2":
                            state = "수업 중";
                            break;
                        case "3":
                            state = "자리 비움";
                            break;
                    }

                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        IndicatorView("[" + name + "]교수님의 현재 상태는 [" + state + "]입니다.");

                        txtSearchName.Text = "";
                    }));
                    
                    break;
                case "00":// 서버 ERROR
                    MessageBox.Show(body);
                    IndicatorView();
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void BtnSearch_Prof(object sender, RoutedEventArgs e)//검색확인버튼
        {
            if (txtSearchName.Text != "")
            {
                CommonData.socketController.SendSearchProf(txtSearchName.Text);
            }
            else
            {
                IndicatorView("교수이름을 올바르게 입력하세요.");
            }
        }

        private void Btn_Cancle(object sender, RoutedEventArgs e)//검색취소버튼
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void IndicatorView(string message = "교수이름을 입력하세요.")
        {
            _context.WorkMessage = message;
        }
    }

    class StateContext2 : INotifyPropertiesBase
    {
        public string WorkMessage { get => workMessage; set { workMessage = value; NotifyPropertyChanged(); } }
        
        private string workMessage = "";
    }
}
