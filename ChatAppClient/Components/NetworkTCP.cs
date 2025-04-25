using System;
using System.Net.Sockets;

namespace ChatAppClient.Components
{
    public static class NetworkTCP
    {
        public static byte[] ReceiveTCP(int size, NetworkStream? sslStream)
        {
            if (sslStream == null)
            {
                throw new NullReferenceException();
            }
            byte[] packet = new byte[size];
            int bytesReceived = 0;
            int x;
            while (bytesReceived < size)
            {
                byte[] buffer = new byte[size - bytesReceived];
                x = sslStream.Read(buffer);
                Buffer.BlockCopy(buffer, 0, packet, bytesReceived, x);
                bytesReceived += x;
            }
            return packet;
        }


        public static void SendTCP(byte[] data, int size, NetworkStream? sslStream)
        {
            if (sslStream == null)
            {
                throw new NullReferenceException();
            }
            sslStream.Write(data, 0, size);
        }

        /// ////////////////////////////////////////////////////////////////////////////////////////////////////// Images functions 

        public static void SendImgTCP(byte[] data, int offset, int size, NetworkStream sslStream)
        {
            if (sslStream == null)
            {
                throw new NullReferenceException();
            }
            sslStream.Write(data, offset, size);
        }


        public static int ReceiveImageTCP(byte[] buffer, int offset, int size, NetworkStream sslStream)
        {
            if (sslStream == null)
            {
                throw new NullReferenceException();
            }
            int bytesReceived = 0;
            while (bytesReceived < size)
            {
                int bytesToReceive = size - bytesReceived;
                int x = sslStream.Read(buffer, offset + bytesReceived, bytesToReceive);
                bytesReceived += x;
                if (x == 0) // connection closed by remote host
                {
                    break;
                }
            }
            return bytesReceived;
        }

    }
}

