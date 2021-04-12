using I_SCADA_CLIENT.common;
using I_SCADA_CLIENT.controller;
using I_SCADA_CLIENT.domain;
using System;
using System.Collections.Generic;
using System.Data;
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

namespace I_SCADA_CLIENT
{
    /// <summary>
    /// MessageWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MessageWindow : Window
    {
        StateContext3 _context = new StateContext3();

        public MessageWindow()
        {
            InitializeComponent();

            this.DataContext = _context;

            /*
            if (CommonData.userSocket != null)
            {
                CommonData.userSocket.Stop();
            }

            CommonData.socketController = new SocketController();
            */
            CommonData.socketController.SocketConnectEvent += SocketController_SocketConnectEvent;
            CommonData.socketController.SocketReceivedMainEvent += SocketController_SocketReceivedMainEvent;
            //CommonData.socketController.ServiceStart();
        }

        private void SocketController_SocketConnectEvent(string type, string address, int port)
        {
            CommonData.socketController.SendLogin();
        }

        private void SocketController_SocketReceivedMainEvent(string head, string body)
        {
            switch (head)
            {
                case "15":// 메세지 조회 Ack
                    Message message = new Message();
                    message.FROM_ID = body.Substring(0, 9);
                    message.FROM_NAME = body.Substring(9, 3);
                    message.MESSAGE = body.Substring(12);

                    List<Message> messages = new List<Message>();
                    for(int i = 0; i < _context.Message_List.Count; i++)
                    {
                        messages.Add(_context.Message_List[i]);
                    }
                    messages.Add(message);

                    _context.Message_List = messages;

                    break;
                case "16":// 메세지 조회 Ack

                    MessageBox.Show("전송완료");

                    _context.ToId = "";
                    _context.ReMessage = "";

                    break;
                case "00":// 서버 ERROR
                    MessageBox.Show(body);
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

        private void Btn_Send_Message(object sender, RoutedEventArgs e)
        {
            try
            {
                if(_context.ToId != "" && _context.ReMessage != "")
                {
                    CommonData.socketController.SendReMessage(_context.ToId, _context.ReMessage);
                }
                else
                {
                    MessageBox.Show("메시지 전송을 위한 To or Message 창을 채우세요.");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Btn_Search_Message(object sender, RoutedEventArgs e)
        {
            try
            {
                CommonData.socketController.SendSearchMessage();
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void DataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                Message dr = (Message)dataGrid.SelectedItem;

                _context.ToId = dr.FROM_ID;

                MessageBox.Show(dr.MESSAGE);
            }
        }
    }

    class StateContext3 : INotifyPropertiesBase
    {
        private List<Message> message_List = new List<Message>();
        public List<Message> Message_List { get => message_List; set { message_List = value; NotifyPropertyChanged(); } }

        private string toId = "";
        public string ToId { get => toId; set { toId = value; NotifyPropertyChanged(); } }

        private string reMessage = "";
        public string ReMessage { get => reMessage; set { reMessage = value; NotifyPropertyChanged(); } }
    }
}
