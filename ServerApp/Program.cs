using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace ServerApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("Сервер запущен. Ожидание подключений...");
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Loopback, 8888);

            Socket sock = new Socket(
                AddressFamily.InterNetwork,
                SocketType.Stream,
                ProtocolType.Tcp
            );

            try
            {
                sock.Bind(ipEndPoint);
                sock.Listen(10);
                Console.WriteLine($"Сервер слушает на {ipEndPoint}");

                while (true)
                {
                    Console.WriteLine("Ожидание подключения...");
                    Socket clientSock = sock.Accept();
                    Console.WriteLine($"Подключен клиент: {clientSock.RemoteEndPoint}");
                    ThreadPool.QueueUserWorkItem(_ => HandleClient(clientSock));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Критическая ошибка: " + ex.Message);
            }
            finally
            {
                sock.Close();
                Console.WriteLine("Сервер остановлен. Нажмите Enter для выхода.");
                Console.ReadLine();
            }
        }

        static void HandleClient(Socket clientSock)
        {
            try
            {
                byte[] bytes = new byte[1024];
                int bytesCount = clientSock.Receive(bytes);

                if (bytesCount == 0)
                {
                    Console.WriteLine("Пустой запрос от клиента");
                    return;
                }

                string data = Encoding.UTF8.GetString(bytes, 0, bytesCount);
                Console.WriteLine($"Получено от клиента: {data}");

                string reply = "Размер запроса: " + data.Length + " символов";
                byte[] msg = Encoding.UTF8.GetBytes(reply);
                clientSock.Send(msg);
                Console.WriteLine($"Отправлен ответ клиенту: {reply}");

                if (data.Contains("<TheEnd>"))
                {
                    Console.WriteLine("Клиент запросил завершение сеанса");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки клиента: {ex.Message}");
            }
            finally
            {
                try
                {
                    clientSock.Shutdown(SocketShutdown.Both);
                    clientSock.Close();
                }
                catch { }
            }
        }
    }
}