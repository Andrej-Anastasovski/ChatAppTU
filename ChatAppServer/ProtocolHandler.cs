// To DO using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatAppServer
{
    public static class ProtocolHandler
    {
        public struct ActiveUser
        {
            public TcpClient client;
            public string profPic;
            public string status;
        };

        // Broadcast active users information to all clients
        public static void BroadcastActiveUsers(Dictionary<string, ActiveUser> clients)
        {
            ActiveUser aUser;
            ActiveUser choosenUser;

            foreach (string user in clients.Keys)
            {
                Send(user, "resetThePannel!", clients);

                choosenUser = clients[user];

                foreach (string username in clients.Keys)
                {
                    aUser = clients[username];

                    if (username != user)
                    {
                        string modifiedUsername = "!@#$" + username;

                        Send(user, modifiedUsername, clients);
                        Send(user, aUser.status, clients);
                        SendLargeImage(aUser.profPic, choosenUser.client.GetStream());
                    }
                }
            }
        }

        // Send data to a specific user
        public static void Send(string username, string packet, Dictionary<string, ActiveUser> clients)
        {
            ActiveUser aUser = clients[username];
            TcpClient client = aUser.client;
            NetworkStream stream = client.GetStream();
            byte[] bytes = Encoding.ASCII.GetBytes(packet);
            byte[] lengthBytes = BitConverter.GetBytes(bytes.Length);
            NetworkTCP.SendTCP(lengthBytes, 4, stream);
            NetworkTCP.SendTCP(bytes, bytes.Length, stream);
        }

        // Receive data from a network stream
        public static string Recieve(NetworkStream stream)
        {
            byte[] lengthBytes = NetworkTCP.ReceiveTCP(4, stream);
            int length = BitConverter.ToInt32(lengthBytes);
            byte[] messageBytes = NetworkTCP.ReceiveTCP(length, stream);
            return Encoding.ASCII.GetString(messageBytes);
        }

        // Send large image to the client
        public static void SendLargeImage(string filePath, NetworkStream sslStream, int chunkSize = 4096)
        {
            try
            {
                byte[] imageData = File.ReadAllBytes(filePath);
                byte[] dataSizeBytes = BitConverter.GetBytes(imageData.Length);
                NetworkTCP.SendTCP(dataSizeBytes, dataSizeBytes.Length, sslStream);

                int bytesSent = 0;
                while (bytesSent < imageData.Length)
                {
                    int bytesToSend = Math.Min(chunkSize, imageData.Length - bytesSent);
                    NetworkTCP.SendImgTCP(imageData, bytesSent, bytesToSend, sslStream);
                    bytesSent += bytesToSend;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending image: " + ex.Message);
            }
        }

        // Send image data as bytes from the server to the client
        public static void SendLargeImageFromBytes(string username, byte[] imageData, Dictionary<string, ActiveUser> clients, int chunkSize = 4096)
        {
            ActiveUser aUser = clients[username];
            TcpClient client = aUser.client;
            NetworkStream sslStream = client.GetStream();

            try
            {
                byte[] dataSizeBytes = BitConverter.GetBytes(imageData.Length);
                NetworkTCP.SendTCP(dataSizeBytes, dataSizeBytes.Length, sslStream);

                int bytesSent = 0;
                while (bytesSent < imageData.Length)
                {
                    int bytesToSend = Math.Min(chunkSize, imageData.Length - bytesSent);
                    NetworkTCP.SendImgTCP(imageData, bytesSent, bytesToSend, sslStream);
                    bytesSent += bytesToSend;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending image: " + ex.Message);
            }
        }

        // Receive large image from the client
        public static byte[] ReceiveLargeImage(NetworkStream sslStream, int chunkSize = 4096)
        {
            byte[] dataSizeBytes = new byte[sizeof(int)];
            int bytesRead = NetworkTCP.ReceiveImageTCP(dataSizeBytes, 0, dataSizeBytes.Length, sslStream);
            int imageDataSize = BitConverter.ToInt32(dataSizeBytes, 0);

            byte[] imageData = new byte[imageDataSize];
            int bytesReceived = 0;
            while (bytesReceived < imageDataSize)
            {
                int bytesToReceive = Math.Min(chunkSize, imageDataSize - bytesReceived);
                bytesReceived += NetworkTCP.ReceiveImageTCP(imageData, bytesReceived, bytesToReceive, sslStream);
            }

            return imageData;
        }

        // Get image byte array from the client
        public static byte[] getImgByteArr(NetworkStream sslStream)
        {
            byte[] dataSizeBytes = NetworkTCP.ReceiveTCP(4, sslStream);
            int dataSize = BitConverter.ToInt32(dataSizeBytes, 0);

            return NetworkTCP.ReceiveTCP(dataSize, sslStream);
        }
    }
}
