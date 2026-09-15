using System.IO.MemoryMappedFiles;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            const string memoryName = "SharedMemory";
            const long memorySize = 2048;
            const int messageLen = 4;
            const int clientOffSet = 1024;

            try
            {

                using (var memory = MemoryMappedFile.OpenExisting(memoryName, MemoryMappedFileRights.ReadWrite))
                {
                    using (var view = memory.CreateViewAccessor(0, memorySize, MemoryMappedFileAccess.ReadWrite))
                    {
                        //read from server
                        var messageLength = new byte[messageLen];
                        view.ReadArray(0, messageLength, 0, messageLen);
                        var length = BitConverter.ToInt32(messageLength, 0);
                        var arr = new byte[length];
                        view.ReadArray(messageLen, arr, 0, length);
                        var message = Encoding.UTF8.GetString(arr);
                        Console.WriteLine($"client received: {message}");


                        //send to server
                        var response = "this is client's response to server";
                        var responseBytes = Encoding.UTF8.GetBytes(response);
                        var responseLength = BitConverter.GetBytes(responseBytes.Length);
                        view.WriteArray(clientOffSet, responseLength, 0, messageLen);
                        view.WriteArray(clientOffSet + messageLen, responseBytes, 0, responseBytes.Length);
                        Console.WriteLine($"client sent: {response}");
                    }
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"an error occured: {ex}");
            }
            finally
            {
                Console.WriteLine("the end of client.");
            }
        }
    }
}
