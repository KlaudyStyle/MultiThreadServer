using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace MultiThreadClient
{
    class Program
    {
        static void Main(string[] args)
        {
            const int PORT = 9595;
            const string SERVER = "127.0.0.1";
            const int CLIENT_COUNT = 5;

            Console.WriteLine("Запуск теста с несколькими клиентами...");

            for (int i = 1; i <= CLIENT_COUNT; i++)
            {
                int clientId = i;
                new Thread(() => RunClient(clientId, SERVER, PORT)).Start();
                Thread.Sleep(100);
            }

            Console.ReadLine();
        }

        static void RunClient(int clientId, string server, int port)
        {
            try
            {
                using (TcpClient client = new TcpClient(server, port))
                using (NetworkStream stream = client.GetStream())
                {
                    string message = $"Привет от клиента {clientId}";
                    byte[] data = Encoding.ASCII.GetBytes(message);

                    stream.Write(data, 0, data.Length);
                    Console.WriteLine($"[Клиент {clientId}] Отправлено: {message}");

                    byte[] buffer = new byte[256];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                    Console.WriteLine($"[Клиент {clientId}] Получено: {response}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Клиент {clientId}] Ошибка: {ex.Message}");
            }
        }
    }
}