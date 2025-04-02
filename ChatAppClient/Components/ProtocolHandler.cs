using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatAppClient.Components
{
    public static class ProtocolHandler
    {
        // Basic method to send data to the client
        public static void Send(string packet, TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            byte[] bytes = Encoding.ASCII.GetBytes(packet);
            int length = bytes.Length;
            byte[] lengthBytes = BitConverter.GetBytes(length);

            stream.Write(lengthBytes, 0, lengthBytes.Length); // Send the length
            stream.Write(bytes, 0, bytes.Length); // Send the actual data
        }

        // Receive a basic message from the stream
        public static string Receive(NetworkStream stream)
        {
            byte[] lengthBytes = new byte[4]; // To store the length of incoming message
            stream.Read(lengthBytes, 0, 4);  // Read the length
            int length = BitConverter.ToInt32(lengthBytes, 0);

            byte[] messageBytes = new byte[length];
            stream.Read(messageBytes, 0, length); // Read the actual message

            return Encoding.ASCII.GetString(messageBytes); // Convert bytes to string and return
        }
    }
}
