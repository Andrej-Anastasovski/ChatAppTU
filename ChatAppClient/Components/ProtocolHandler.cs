using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

namespace ChatAppClient.Components
{
    public static class ProtocolHandler
    {

        public static void Send(string packet, TcpClient client)
        {
            // Client that is being transmited to!
            NetworkStream stream = client.GetStream();
            byte[] bytes = Encoding.ASCII.GetBytes(packet); // Client username that is beeing sent! 

            /// lenght first!
            int length = bytes.Length;
            byte[] leghtbytes = BitConverter.GetBytes(length);
            NetworkTCP.SendTCP(leghtbytes, 4, stream);
            NetworkTCP.SendTCP(bytes, length, stream);

            // stream.Write(bytes, 0, bytes.Length);
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

        public static string Recieve(NetworkStream stream)
        {
            byte[] lengthBytes = NetworkTCP.ReceiveTCP(4, stream);
            int length = BitConverter.ToInt32(lengthBytes);
            byte[] messageBytes = NetworkTCP.ReceiveTCP(length, stream);
            string message = Encoding.ASCII.GetString(messageBytes);
            return message;
        }



    }
}

