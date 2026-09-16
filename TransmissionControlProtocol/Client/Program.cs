using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const int serverPort = 5000;

            using (var socket = new TcpClient())
            {
                await socket.ConnectAsync(IPAddress.Loopback, serverPort);

                Console.WriteLine("client connected!");

                using (var stream = socket.GetStream())
                {
                    var message = "hi server. Im client. can you explain about tcp?";
                    var bytes = Encoding.UTF8.GetBytes(message);
                    await stream.WriteAsync(bytes);

                    var buffer = new byte[1024];
                    while (true)
                    {
                        var bytesRead = await stream.ReadAsync(buffer);
                        if (bytesRead == 0)
                        {
                            break;
                        }

                        var incomingMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"client received: {incomingMessage}");
                    }
                    
                }

                Console.ReadLine();
            }
        }
    }
}
