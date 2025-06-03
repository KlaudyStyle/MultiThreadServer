using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MultiThreadServer
{
    class Program
    {
        static void Main(string[] args)
        {
            TcpListener server = null;
            try
            {
                int maxThreads = Environment.ProcessorCount * 4;
                ThreadPool.SetMaxThreads(maxThreads, maxThreads);
                ThreadPool.SetMinThreads(2, 2);

                int port = 9595;
                IPAddress localAddr = IPAddress.Parse("127.0.0.1");
                int counter = 0;

                server = new TcpListener(localAddr, port);
                server.Start();

                Console.WriteLine("Конфигурация сервера:");
                Console.WriteLine($"IP-адрес: {localAddr}");
                Console.WriteLine($"Порт: {port}");
                Console.WriteLine($"Макс. потоков: {maxThreads}");
                Console.WriteLine("\nСервер запущен. Ожидание подключений...");

                while (true)
                {
                    Console.WriteLine("\nОжидание подключения...");
                    TcpClient client = server.AcceptTcpClient();

                    ThreadPool.QueueUserWorkItem(ClientProcessing, client);
                    counter++;
                    Console.WriteLine($"Подключение #{counter} принято!");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
            finally
            {
                server?.Stop();
                Console.WriteLine("\nСервер остановлен. Нажмите Enter для выхода...");
                Console.ReadLine();
            }
        }

        static void ClientProcessing(object clientObj)
        {
            TcpClient client = (TcpClient)clientObj;
            NetworkStream stream = null;

            try
            {
                stream = client.GetStream();
                byte[] buffer = new byte[256];
                int bytesRead;

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    string data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Получено: {data}");

                    string response = data.ToUpper();
                    byte[] msg = Encoding.ASCII.GetBytes(response);

                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine($"Отправлено: {response}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки: {ex.Message}");
            }
            finally
            {
                stream?.Close();
                client?.Close();
            }
        }
    }
}