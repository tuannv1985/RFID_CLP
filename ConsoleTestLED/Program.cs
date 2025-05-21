using Newtonsoft.Json;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleTestLED
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");

            

            // Send via TCP
            string server = "113.161.77.106";
            int port = 8888;

            while (true)
            {
                string html = $"<div style=\"width:100%;height:100%;background-color:yellow\"><p style=\"color:red;\">{DateTime.Now:HH:mm:ss}</p></div>";
                var payload = new
                {
                    text = html,
                    sign = ComputeSHA256(html)
                };

                string json = JsonConvert.SerializeObject(payload);
                Console.WriteLine("Sending: " + json);

                try
                {
                    using TcpClient client = new TcpClient(server, port);
                    using NetworkStream stream = client.GetStream();
                    byte[] data = Encoding.UTF8.GetBytes(json + "\n");

                    // Send the JSON
                    stream.Write(data, 0, data.Length);
                    Console.WriteLine("Sent JSON to server.");

                    // Optional: Read response
                    byte[] buffer = new byte[4096];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Console.WriteLine($"Received: {response}");
                    Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error: {ex.Message}");
                }
            }

            Console.ReadLine();
        }
        static string ComputeSHA256(string input)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
        }
    }
}
