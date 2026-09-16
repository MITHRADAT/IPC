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
            const int clientPort = 5001;

            using (var socket = new UdpClient(serverPort))
            {
                var incoming = await socket.ReceiveAsync();
                var incomingMessage = Encoding.UTF8.GetString(incoming.Buffer);
                Console.WriteLine($"server received: {incomingMessage}");
                
                var clientEndPoint = new IPEndPoint(IPAddress.Loopback, clientPort);

                var datagrams = new[]
                {
                    GetDatagram("udp is user datagram protocol"),
                    GetDatagram("another way for inter process communication"),
                    GetDatagram("it has limitation of size about 65 KB"),
                    GetDatagram("but its faster than tcp because of its thinner interface and allowing user to implement his own logic"),
                    GetDatagram("there is no guarantee of delivery of datagrams and order of delivery"),
                };

                var tasks = datagrams.Select(d => socket.SendAsync(d, clientEndPoint).AsTask());

                await Task.WhenAll(tasks);

                Console.WriteLine($"server sent its own datagram frames");

                Console.ReadLine();
            }
        }

        static byte[] GetDatagram(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
