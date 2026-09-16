using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const int serverPort = 5000;

            using (var listener = new TcpListener(IPAddress.Loopback, serverPort))
            {
                listener.Start();
                Console.WriteLine("server is listening...");

                using (var socket = await listener.AcceptTcpClientAsync())
                using (var stream = socket.GetStream())
                {
                    Console.WriteLine("client connected!");

                    var buffer = new byte[1024];
                    var bytesRead = await stream.ReadAsync(buffer);
                    var incomingMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"server received: {incomingMessage}");

                    var bytes = new[]
                    {
                        GetBytes("tcp is transmission control protocol"),
                        GetBytes("tcp provides a reliable ordered byte stream"),
                        GetBytes("tcp does not preserve message boundaries"),
                        GetBytes("tcp has no limitation for size of messages"),
                        GetBytes("tcp retransmits lost data and guarantees ordered delivery")
                    };

                    var tasks = bytes.Select(b => stream.WriteAsync(b).AsTask());

                    await Task.WhenAll(tasks);

                    Console.WriteLine("server completed sending stream");
                    Console.ReadLine();
                }

                listener.Stop();
            }
        }

        private static byte[] GetBytes(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
