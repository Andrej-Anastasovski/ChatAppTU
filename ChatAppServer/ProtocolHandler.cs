using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static ChatAppServer.Server;

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

        public static void BroadcastActiveUsers(Dictionary<string, ActiveUser> clients) // Function to send info to all clients ( used for activeUsers ) 
        {
            ActiveUser aUser;
            ActiveUser choosenUser;

            foreach (string user in clients.Keys) // For every active client/user
            {


                Send(user, "resetThePannel!", clients);

                choosenUser = clients[user];

                foreach (string username in clients.Keys) // All active clients/users
                {
                    aUser = clients[username];


                    if (username != user) // Except the one that is beeing transmitted to
                    {

                        Console.WriteLine("StackPannel Send: {0} to user {1}", username, user);
                        string modifiedUsername = "!@#$" + username;

                        //// Broad cast Users treba da prasta sliki 
                        Send(user, modifiedUsername, clients);
                        Send(user, aUser.status, clients);
                        SendLargeImage(aUser.profPic, choosenUser.client.GetStream());



                    }
                }

            }
        }



        public static void Send(string username, string packet, Dictionary<string, ActiveUser> clients)
        {
            ActiveUser aUser;
            aUser = clients[username];

            TcpClient client = aUser.client; // Client that is beeing transmited to!
            NetworkStream stream = client.GetStream();
            byte[] bytes = System.Text.Encoding.ASCII.GetBytes(packet); // Client username that is beeing sent! 
            int length = bytes.Length;
            byte[] leghtbytes = BitConverter.GetBytes(length);
            NetworkTCP.SendTCP(leghtbytes, 4, stream);
            NetworkTCP.SendTCP(bytes, length, stream);
        }

        public static string Recieve(NetworkStream stream)
        {
            byte[] lengthBytes = NetworkTCP.ReceiveTCP(4, stream);
            int length = BitConverter.ToInt32(lengthBytes);
            byte[] messageBytes = NetworkTCP.ReceiveTCP(length, stream);
            string message = Encoding.ASCII.GetString(messageBytes);
            return message;
        }

        public static void SendLargeImage(string filePath, NetworkStream sslStream, int chunkSize = 4096)
        {
            try
            {
                // Load the image into a byte array
                byte[] imageData = File.ReadAllBytes(filePath);

                // Send the size of the image data to the server
                byte[] dataSizeBytes = BitConverter.GetBytes(imageData.Length);
                NetworkTCP.SendTCP(dataSizeBytes, dataSizeBytes.Length, sslStream);

                // Send the image data to the server in chunks
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



        public static void SendLargeImageFromBytes(string username, byte[] imageData, Dictionary<string, ActiveUser> clients, int chunkSize = 4096)
        {

            ActiveUser aUser;
            aUser = clients[username];


            TcpClient client = aUser.client; // Client that is beeing transmited to!
            NetworkStream sslStream = client.GetStream();



            try
            {

                // Send the size of the image data to the server
                byte[] dataSizeBytes = BitConverter.GetBytes(imageData.Length);
                NetworkTCP.SendTCP(dataSizeBytes, dataSizeBytes.Length, sslStream);

                // Send the image data to the server in chunks
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


        public static byte[] ReceiveLargeImage(NetworkStream sslStream, int chunkSize = 4096)
        {

            // Receive the size of the image data from the server
            byte[] dataSizeBytes = new byte[sizeof(int)];
            int bytesRead = NetworkTCP.ReceiveImageTCP(dataSizeBytes, 0, dataSizeBytes.Length, sslStream);
            int imageDataSize = BitConverter.ToInt32(dataSizeBytes, 0);

            // Receive the image data from the server in chunks
            byte[] imageData = new byte[imageDataSize];
            int bytesReceived = 0;
            while (bytesReceived < imageDataSize)
            {
                int bytesToReceive = Math.Min(chunkSize, imageDataSize - bytesReceived);
                bytesReceived += NetworkTCP.ReceiveImageTCP(imageData, bytesReceived, bytesToReceive, sslStream);
            }

            // Save the image to the specified file path
            return imageData;


        }

        public static byte[] getImgByteArr(NetworkStream sslStream)
        {
            // Read the size of the image data from the client
            byte[] dataSizeBytes = NetworkTCP.ReceiveTCP(4, sslStream);
            int dataSize = BitConverter.ToInt32(dataSizeBytes, 0);

            // Read the image data from the client
            byte[] imageData = NetworkTCP.ReceiveTCP(dataSize, sslStream);

            return imageData;

        }
    }
}
