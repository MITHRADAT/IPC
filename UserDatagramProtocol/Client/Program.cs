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
            const int clientPort = 5001;

            using (var socket = new UdpClient(clientPort))
            {
                var serverEndPoint = new IPEndPoint(IPAddress.Loopback, serverPort);

                var message = "hi server. im client. can you explain about udp?";
                var datagram = Encoding.UTF8.GetBytes(message);
                await socket.SendAsync(datagram, serverEndPoint);

                while (true)
                {
                    var incomingDatagram = await socket.ReceiveAsync();
                    var incomingMessage = Encoding.UTF8.GetString(incomingDatagram.Buffer);

                    Console.WriteLine($"client received: {incomingMessage}");
                }
            }
        }
    }
}
