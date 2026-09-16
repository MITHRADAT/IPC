using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            const int port = 5000;
            var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            Console.WriteLine($"http server is listening on http://localhost:{port}");
            while (true)
            {
                using (var client = await listener.AcceptTcpClientAsync())
                using (var stream = client.GetStream())
                {
                    var buffer = new byte[4096];
                    var bytesRead = await stream.ReadAsync(buffer);
                    var request = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    Console.WriteLine($"request is {request}");

                    if (request.StartsWith("GET /explain"))
                    {
                        var body = """
                           HTTP stands for Hypertext Transfer Protocol.

                           it is an application-layer protocol used for communication between clients and servers.

                           tcp provides the reliable byte stream underneath http,
                           while http gives those bytes meaning through 
                           requests, responses, methods, headers, status codes, and bodies.

                           otherwise, we might receive "how are you?" stream as "howa reyo u ?" or anything else.
                           but http can provide a protocol to know what is what in tcp layer.
                           """;

                        var bodyBytes = Encoding.UTF8.GetBytes(body);

                        var header =
                            $"HTTP/1.1 200 OK\r\n" +
                            $"Content-Type: text/plain; charset=utf-8\r\n" +
                            $"Content-Length: {bodyBytes.Length}\r\n" +
                            $"\r\n";

                        var headerBytes = Encoding.UTF8.GetBytes(header);

                        await stream.WriteAsync(headerBytes);
                        await stream.WriteAsync(bodyBytes);
                    }
                    else
                    {
                        var body = "Not Found";

                        var bodyBytes = Encoding.UTF8.GetBytes(body);

                        var header =
                            $"HTTP/1.1 404 Not Found\r\n" +
                            $"Content-Type: text/plain; charset=utf-8\r\n" +
                            $"Content-Length: {bodyBytes.Length}\r\n" +
                            $"\r\n";

                        var headerBytes = Encoding.UTF8.GetBytes(header);

                        await stream.WriteAsync(headerBytes);
                        await stream.WriteAsync(bodyBytes);
                    }
                }
            }
        }
    }
}
