using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ChatAppServer
{
    class Server
    {
        static Dictionary<string, ProtocolHandler.ActiveUser> clients = new Dictionary<string, ProtocolHandler.ActiveUser>();

        static void Main(string[] args)
        {
            // TODO: Initialize server
            TcpListener chatServer = null;
            try
            {
                Int32 port = 10069;
                IPAddress localAddress = IPAddress.Parse("127.0.0.1");
                chatServer = new TcpListener(localAddress, port);
                chatServer.Start();

                // TODO: Start listening thread
                Thread listenThread = new Thread(() => ListenForClients(chatServer));
                listenThread.Start();

                Console.WriteLine("Server started on {0}:{1}", localAddress, port);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket Exception: {0}", e);
            }
        }

        static void ListenForClients(TcpListener server)
        {
            // TODO: Accept incoming client connections and handle them in new threads
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }

        static void HandleClient(TcpClient client)
        {
            NetworkStream stream = client.GetStream();
            string profPicName = "defaultProf.jpg";
            string projectFolderPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string currClientUsername = "";

            List<accountModel> data = SqliteDataAccess.loadPeople();

            // TODO: Handle client authentication
            try
            {
                string username = ProtocolHandler.Recieve(stream);
                string password = ProtocolHandler.Recieve(stream);

                // TODO: Registration Flow
                if (username.Substring(0, 3) == "reg" && password.Substring(0, 3) == "reg")
                {
                    // TODO: Handle registration logic
                    username = username.Remove(0, 3);
                    password = password.Remove(0, 3);

                    accountModel a = new accountModel(username, password, profPicName, "Active");

                    // TODO: Check if username exists and save if not
                    if (data.Any(x => x.username == username))
                    {
                        throw new Exception("Username already exists in database");
                    }

                    SqliteDataAccess.SaveAccount(a);

                    // Retry login after registration
                    goto tryagain;
                }
                else
                {
                    // TODO: Login Flow
                    bool foundUser = false;
                    data = SqliteDataAccess.loadPeople();

                    foreach (accountModel account in data)
                    {
                        if (account.username == username && account.password == password)
                        {
                            string newProfPic = account.profPic;

                            ProtocolHandler.ActiveUser newUser = new ProtocolHandler.ActiveUser();
                            newUser.client = client;
                            newUser.profPic = newProfPic;
                            newUser.status = account.status;

                            string profPicPath = Path.Combine(projectFolderPath, "profPictures", newProfPic);
                            clients[username] = newUser;

                            ProtocolHandler.Send(username, "profPicset", clients);
                            ProtocolHandler.SendLargeImage(profPicPath, stream);

                            foundUser = true;
                            currClientUsername = username;
                            break;
                        }
                    }

                    if (!foundUser)
                    {
                        // TODO: Handle invalid login
                        byte[] bytes = Encoding.ASCII.GetBytes("FailedToSignIn");
                        int length = bytes.Length;
                        byte[] lengthBytes = BitConverter.GetBytes(length);
                        NetworkTCP.SendTCP(lengthBytes, 4, stream);
                        NetworkTCP.SendTCP(bytes, length, stream);

                        goto tryagain;
                    }
                }
            }
            catch
            {
                Console.WriteLine("Client disconnected without logging in.");
            }

            // TODO: Broadcast active users if more than 1
            if (clients.Count > 1)
            {
                ProtocolHandler.BroadcastActiveUsers(clients);
            }

            try
            {
                while (true)
                {
                    // TODO: Handle incoming messages
                    string message = ProtocolHandler.Recieve(stream);

                    if (message == "####")
                    {
                        // TODO: Handle client disconnection
                        clients.Remove(currClientUsername);
                        ProtocolHandler.BroadcastActiveUsers(clients);
                        break;
                    }
                    else if (message == "msgInComming")
                    {
                        // TODO: Handle incoming message
                        string toUsername = ProtocolHandler.Recieve(stream);
                        string sendMessage = ProtocolHandler.Recieve(stream);

                        ProtocolHandler.Send(toUsername, "recieveMsg", clients);
                        ProtocolHandler.Send(toUsername, currClientUsername, clients);
                        ProtocolHandler.Send(toUsername, sendMessage, clients);
                    }
                    else if (message == "largeImageIncomming")
                    {
                        // TODO: Handle large image transfer
                        string toUsername = ProtocolHandler.Recieve(stream);
                        byte[] imgData = ProtocolHandler.ReceiveLargeImage(stream);

                        ProtocolHandler.Send(toUsername, "recieveAUUUBAUUU", clients);
                        ProtocolHandler.Send(toUsername, currClientUsername, clients);
                        ProtocolHandler.SendLargeImageFromBytes(toUsername, imgData, clients);
                    }
                    else if (message == "emojiSend")
                    {
                        // TODO: Handle emoji transfer
                        string toUsername = ProtocolHandler.Recieve(stream);
                        ProtocolHandler.Send(toUsername, "newEmoji", clients);
                        ProtocolHandler.Send(toUsername, currClientUsername, clients);
                    }
                    else
                    {
                        // TODO: Handle other messages (if needed)
                    }
                }
            }
            catch
            {
                Console.WriteLine("{0} has disconnected.", currClientUsername);
                clients.Remove(currClientUsername);
                ProtocolHandler.BroadcastActiveUsers(clients);
            }

            client.Close();
        }
    }
}
 