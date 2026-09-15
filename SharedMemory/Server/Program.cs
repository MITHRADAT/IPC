using System.IO.MemoryMappedFiles;
using System.Text;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string memoryName = "SharedMemory";
            const long memorySize = 2048;
            const int messageLen = 4;
            const int clientOffSet = 1024;

            using (var memory = MemoryMappedFile.CreateOrOpen(memoryName, memorySize, MemoryMappedFileAccess.ReadWrite))
            {
                using (var view = memory.CreateViewAccessor(0, memorySize, MemoryMappedFileAccess.ReadWrite))
                {
                    //send to client
                    var message = "this message is from server. i love programming :)";
                    var arr = Encoding.UTF8.GetBytes(message);
                    var messageLength = BitConverter.GetBytes(arr.Length);
                    view.WriteArray(0, messageLength, 0, messageLen);
                    view.WriteArray(messageLen, arr, 0, arr.Length);
                    Console.WriteLine($"sever sent: {message}");

                    
                    //read from client
                    Thread.Sleep(10000); //this is awful because there shared memory is not a good practice for two way communication because there is no synchronization
                    var responseLengthBytes = new byte[messageLen];
                    view.ReadArray(clientOffSet, responseLengthBytes, 0, messageLen);
                    var responseLength = BitConverter.ToInt32(responseLengthBytes, 0);
                    var responseBytes = new byte[responseLength];
                    view.ReadArray(clientOffSet + messageLen, responseBytes, 0, responseLength);
                    var response = Encoding.UTF8.GetString(responseBytes);
                    Console.WriteLine($"server received: {response}");
                    
                    
                    
                    Console.ReadLine();
                }
            }
        }
    }
}
