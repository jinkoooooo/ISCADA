using I_SCADA_CLIENT.common;
using I_SCADA_CLIENT.controller;
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
    /// ScadaWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ScadaWindow : Window
    {
        StateContext3 _context = new StateContext3();

        public bool _threadFlag = false;

        public ScadaWindow()
        {
            InitializeComponent();

            this.DataContext = _context;

            /*
            if (CommonData.userSocket != null)
            {
                CommonData.userSocket.Stop();
            }
            
            CommonData.socketController = new SocketController();*/
            CommonData.socketController.SocketConnectEvent += SocketController_SocketConnectEvent;
            CommonData.socketController.SocketReceivedMainEvent += SocketController_SocketReceivedMainEvent;
            //CommonData.socketController.ServiceStart();
            _threadFlag = true;

            Thread ScadaSocketRunning = new Thread(ScadaRunning);
            ScadaSocketRunning.Start();
        }

        private void ScadaRunning()
        {
            while (_threadFlag)
            {
                try
                {
                    CommonData.socketController.SendSearchAllState();//전체 상태 조회

                    Thread.Sleep(1000);//딜레이 1초
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }
            }
        }

        private void SocketController_SocketConnectEvent(string type, string address, int port)
        {
            CommonData.socketController.SendLogin();
        }

        private void SocketController_SocketReceivedMainEvent(string head, string body)
        {
            switch (head)
            {
                case "21":// Scada 조회 Ack

                    string status = body.Substring(0, 1);
                    string address = body.Substring(1);
                    //상담가능 상담중 수업중 자리비움
                    Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate
                    {
                        switch (address)
                        {
                            case "07301":
                                if (status == "0") room07301.Fill = state1.Fill;
                                else if (status == "1") room07301.Fill = state2.Fill;
                                else if (status == "2") room07301.Fill = state3.Fill;
                                else if (status == "3") room07301.Fill = state4.Fill;

                                break;
                            case "07302":
                                if (status == "0") room07302.Fill = state1.Fill;
                                else if (status == "1") room07302.Fill = state2.Fill;
                                else if (status == "2") room07302.Fill = state3.Fill;
                                else if (status == "3") room07302.Fill = state4.Fill;
                                break;
                            case "07303":
                                if (status == "0") room07303.Fill = state1.Fill;
                                else if (status == "1") room07303.Fill = state2.Fill;
                                else if (status == "2") room07303.Fill = state3.Fill;
                                else if (status == "3") room07303.Fill = state4.Fill;
                                break;
                            case "07304":
                                if (status == "0") room07304.Fill = state1.Fill;
                                else if (status == "1") room07304.Fill = state2.Fill;
                                else if (status == "2") room07304.Fill = state3.Fill;
                                else if (status == "3") room07304.Fill = state4.Fill;
                                break;
                            case "07305":
                                if (status == "0") room07305.Fill = state1.Fill;
                                else if (status == "1") room07305.Fill = state2.Fill;
                                else if (status == "2") room07305.Fill = state3.Fill;
                                else if (status == "3") room07305.Fill = state4.Fill;
                                break;
                            case "07401":
                                if (status == "0") room07401.Fill = state1.Fill;
                                else if (status == "1") room07401.Fill = state2.Fill;
                                else if (status == "2") room07401.Fill = state3.Fill;
                                else if (status == "3") room07401.Fill = state4.Fill;
                                break;
                            case "07402":
                                if (status == "0") room07402.Fill = state1.Fill;
                                else if (status == "1") room07402.Fill = state2.Fill;
                                else if (status == "2") room07402.Fill = state3.Fill;
                                else if (status == "3") room07402.Fill = state4.Fill;
                                break;
                            case "07403":
                                if (status == "0") room07403.Fill = state1.Fill;
                                else if (status == "1") room07403.Fill = state2.Fill;
                                else if (status == "2") room07403.Fill = state3.Fill;
                                else if (status == "3") room07403.Fill = state4.Fill;
                                break;
                            case "07404":
                                if (status == "0") room07404.Fill = state1.Fill;
                                else if (status == "1") room07404.Fill = state2.Fill;
                                else if (status == "2") room07404.Fill = state3.Fill;
                                else if (status == "3") room07404.Fill = state4.Fill;
                                break;
                            case "07405":
                                if (status == "0") room07405.Fill = state1.Fill;
                                else if (status == "1") room07405.Fill = state2.Fill;
                                else if (status == "2") room07405.Fill = state3.Fill;
                                else if (status == "3") room07405.Fill = state4.Fill;
                                break;
                            case "07501":
                                if (status == "0") room07501.Fill = state1.Fill;
                                else if (status == "1") room07501.Fill = state2.Fill;
                                else if (status == "2") room07501.Fill = state3.Fill;
                                else if (status == "3") room07501.Fill = state4.Fill;
                                break;
                            case "07502":
                                if (status == "0") room07502.Fill = state1.Fill;
                                else if (status == "1") room07502.Fill = state2.Fill;
                                else if (status == "2") room07502.Fill = state3.Fill;
                                else if (status == "3") room07502.Fill = state4.Fill;
                                break;
                            case "07503":
                                if (status == "0") room07503.Fill = state1.Fill;
                                else if (status == "1") room07503.Fill = state2.Fill;
                                else if (status == "2") room07503.Fill = state3.Fill;
                                else if (status == "3") room07503.Fill = state4.Fill;
                                break;
                            case "07504":
                                if (status == "0") room07504.Fill = state1.Fill;
                                else if (status == "1") room07504.Fill = state2.Fill;
                                else if (status == "2") room07504.Fill = state3.Fill;
                                else if (status == "3") room07504.Fill = state4.Fill;
                                break;
                            case "07505":
                                if (status == "0") room07505.Fill = state1.Fill;
                                else if (status == "1") room07505.Fill = state2.Fill;
                                else if (status == "2") room07505.Fill = state3.Fill;
                                else if (status == "3") room07505.Fill = state4.Fill;
                                break;
                        }
                    }));
                    

                    break;
                case "00":// 서버 ERROR
                    MessageBox.Show(body);
                    break;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            _threadFlag = false;// 빽 쓰래드 종료

            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Rectangle_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
