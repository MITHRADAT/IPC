using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        static List<int> Numbers = new() { 1, 3, 5, 6, 7, 10, 24 };

        static async Task Main(string[] args)
        {
            const int port = 5000;

            var listener = new TcpListener(IPAddress.Loopback, port);

            listener.Start();

            Console.WriteLine($"REST server is listening on http://localhost:{port}");

            while (true)
            {
                using var client = await listener.AcceptTcpClientAsync();
                using var stream = client.GetStream();

                var buffer = new byte[4096];

                var bytesRead = await stream.ReadAsync(buffer);

                var request = Encoding.UTF8.GetString(
                    buffer,
                    0,
                    bytesRead);

                Console.WriteLine($"request:\n{request}");

                var requestLine = request.Split("\r\n")[0];

                var parts = requestLine.Split(' ');

                if (parts.Length < 2)
                {
                    await SendResponse(
                        stream,
                        "400 Bad Request",
                        "Invalid HTTP request.");

                    continue;
                }

                var method = parts[0];
                var path = parts[1];

                var response = HandleRequest(method, path);

                await SendResponse(stream, response.Status, response.Body);
            }
        }

        static Response HandleRequest(string method, string path)
        {
            var parts = path.Trim('/').Split('/');

            if (method == "GET" && parts.Length == 1 && parts[0] == "explain")
            {
                var body = """
                           REST stands for REpresentational State Transfer.
                           REST is architectural style.
                           REST is heavily around the concept of resources.
                           REST makes HTTP methods meaningful.
                           REST requires statelessness, meaning each request is independent from other requests, so load balancers can help scaling.
                           REST can use HTTP header more meaningful, where document type and status codes could be more specific
                                for example specifying document type in header or more detailed status code

                           finally, REST answers the question: "how to design an app/api around http so that clients and servers interact through a consistent architecture?"

                           so REST is not an API returning JSON.
                           """;

                return new Response("200 OK", body);
            }

            // /numbers
            if (parts.Length == 1 && parts[0] == "numbers")
            {
                return method switch
                {
                    "GET" => new Response("200 OK", string.Join(", ", Numbers)),

                    "POST" => new Response("501 Not Implemented", "POST body parsing not implemented yet."),

                    _ => new Response("405 Method Not Allowed", "Method not supported for /numbers.")
                };
            }

            // /numbers/{index}
            if (parts.Length == 2 && parts[0] == "numbers")
            {
                if (!int.TryParse(parts[1], out var index))
                {
                    return new Response("400 Bad Request", "Invalid number index.");
                }

                if (index < 0 || index >= Numbers.Count)
                {
                    return new Response("404 Not Found", "Number not found.");
                }

                return method switch
                {
                    "GET" => new Response("200 OK", Numbers[index].ToString()),

                    "DELETE" => DeleteNumber(index),

                    "PUT" => new Response("501 Not Implemented", "PUT body parsing not implemented yet."),

                    _ => new Response("405 Method Not Allowed", "Method not supported for this resource.")
                };
            }

            return new Response("404 Not Found", "Resource not found.");
        }

        static Response DeleteNumber(int index)
        {
            Numbers.RemoveAt(index);

            return new Response("204 No Content", "");
        }

        static async Task SendResponse(NetworkStream stream, string status, string body)
        {
            var bodyBytes = Encoding.UTF8.GetBytes(body);

            var header =
                $"HTTP/1.1 {status}\r\n" +
                $"Content-Type: text/plain; charset=utf-8\r\n" +
                $"Content-Length: {bodyBytes.Length}\r\n" +
                $"Connection: close\r\n" +
                $"\r\n";

            var headerBytes = Encoding.UTF8.GetBytes(header);

            await stream.WriteAsync(headerBytes);
            await stream.WriteAsync(bodyBytes);
        }

        record Response(string Status, string Body);
    }
}