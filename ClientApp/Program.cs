using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace ClientApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Communicate("localhost", 8888);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка: " + ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }

        static void Communicate(string hostname, int port)
        {
            byte[] bytes = new byte[1024];

            IPHostEntry ipHost = Dns.GetHostEntry(hostname);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);

            Socket sock = new Socket(
                ipAddr.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            sock.Connect(ipEndPoint);
            Console.Write("Введите сообщение: ");
            string message = Console.ReadLine();

            byte[] data = Encoding.UTF8.GetBytes(message);
            int bytesSent = sock.Send(data);

            int bytesRec = sock.Receive(bytes);
            Console.WriteLine("Ответ сервера: {0}\n",
                Encoding.UTF8.GetString(bytes, 0, bytesRec));

            if (!message.Contains("<TheEnd>"))
                Communicate(hostname, port);

            sock.Shutdown(SocketShutdown.Both);
            sock.Close();
        }
    }
}