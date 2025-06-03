using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows;

namespace WpfClientApp
{
    public partial class MainWindow : Window
    {
        private const int PORT = 8888;

        public MainWindow()
        {
            InitializeComponent();
            Title = "Клиент (подключение к порту " + PORT;
        }

        private void btnSend_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string message = txtMessage.Text;
                if (string.IsNullOrWhiteSpace(message))
                {
                    MessageBox.Show("Введите сообщение");
                    return;
                }

                using (Socket clientSocket = new Socket(
                    AddressFamily.InterNetwork,
                    SocketType.Stream,
                    ProtocolType.Tcp))
                {
                    IPHostEntry ipHost = Dns.GetHostEntry("localhost");
                    IPAddress ipAddr = ipHost.AddressList[0];
                    IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Loopback, PORT);

                    IAsyncResult result = clientSocket.BeginConnect(ipEndPoint, null, null);
                    bool success = result.AsyncWaitHandle.WaitOne(5000, true);

                    if (!success || !clientSocket.Connected)
                    {
                        MessageBox.Show("Ошибка подключения к серверу. Таймаут.");
                        return;
                    }
                    byte[] data = Encoding.UTF8.GetBytes(message);
                    int bytesSent = clientSocket.Send(data);
                    txtResponse.Text += $"[Отправлено {bytesSent} байт]\n";

                    clientSocket.ReceiveTimeout = 10000;
                    byte[] buffer = new byte[1024];
                    int bytesRec = clientSocket.Receive(buffer);

                    if (bytesRec > 0)
                    {
                        string response = Encoding.UTF8.GetString(buffer, 0, bytesRec);
                        txtResponse.Text += $"[Ответ сервера]: {response}\n";
                    }
                    else
                    {
                        txtResponse.Text += "[Сервер не ответил]\n";
                    }

                    if (message.Contains("<TheEnd>"))
                    {
                        Close();
                    }
                }
            }
            catch (SocketException sex)
            {
                txtResponse.Text += $"[Ошибка сокета]: {sex.SocketErrorCode} - {sex.Message}\n";
            }
            catch (Exception ex)
            {
                txtResponse.Text += $"[Ошибка]: {ex.Message}\n";
            }
        }
    }
}