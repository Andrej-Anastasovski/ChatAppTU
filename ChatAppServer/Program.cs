using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.ComponentModel.DataAnnotations;
using Dapper;

namespace ChatAppServer
{

    class Server
    {

        static Dictionary<string, ProtocolHandler.ActiveUser> clients = new Dictionary<string, ProtocolHandler.ActiveUser>();


        

        static void Main(string[] args) ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        {
            TcpListener chatServer = null;
            try 
            {
                Int32 port = 10069;
                IPAddress localAddress = IPAddress.Parse("127.0.0.1");
                chatServer = new TcpListener(localAddress, port); 
                chatServer.Start();


                //New Thread to handle the incomming client connection requests
                Thread listenThread = new Thread(() => ListenForClients(chatServer));
                listenThread.Start();

                Console.WriteLine("Server has started successfully on {0}:{1} ", localAddress, port);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket Exception: {0}", e);
            }

        }

        static void ListenForClients(TcpListener server) ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        {
            while (true)
            {
                // Accept incoming client connections.
                TcpClient client = server.AcceptTcpClient();

                // Start a new thread to handle the new client.
                Thread clientThread = new Thread(() => HandleClient(client));
                clientThread.Start();
            }
        }

        static void HandleClient(TcpClient client) //////////////////////////////////HANDLEEEEEEEEEEEEEEEEEEEEEEEE///////////////////////////////////////////////////////
        {
           
            NetworkStream stream = client.GetStream();

            Console.WriteLine("A client has been connected to the server");


            string profPicName = "defaultProf.jpg";
            string projectFolderPath = Directory.GetParent(Environment.CurrentDirectory).Parent.Parent.FullName;
            string currClientUsername = "";


            List<accountModel> data = SqliteDataAccess.loadPeople();

            if (data.Count > 0)
            {
                accountModel r = data[0];
                Console.WriteLine("The first account in the database is username: {0}, password: {1}", r.username, r.password);
            }
            else
            {
                Console.WriteLine("There are no accounts in the database.");
            }


            try
            {


            tryagain:

                string username = ProtocolHandler.Recieve(stream);
                string password = ProtocolHandler.Recieve(stream);

                if (username.Substring(0, 3) == "reg" && password.Substring(0, 3) == "reg")
                {
                    username = username.Remove(0, 3);
                    password = password.Remove(0, 3);


                    accountModel a = new accountModel(username, password, profPicName, "Active");


                    // check if username already exists in database
                    if (data.Any(x => x.username == username))
                    {
                        throw new Exception("Username already exists in database");
                    }


                    SqliteDataAccess.SaveAccount(a);

                    goto tryagain;
                }
                else
                {
                    bool foundUser = false;
                    data = SqliteDataAccess.loadPeople(); // Refresh the new registration 


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

                            Console.WriteLine("{0} has logged in successfully!", username);

                            clients[username] = newUser;

                            ProtocolHandler.Send(username, "profPicset", clients);

                            ProtocolHandler.SendLargeImage(profPicPath, stream);

                            Console.WriteLine("prof Pic has been sent: {0}", newProfPic);

                            foundUser = true;

                            currClientUsername = username;

                            break;
                        }
                    }



                    if (!foundUser)
                    {
                        Console.WriteLine("Invalid username or password.");
                        Console.WriteLine("Please try again.");

                        byte[] bytes = System.Text.Encoding.ASCII.GetBytes("FailedToSignIn"); // Client username that is beeing sent! 
                        int length = bytes.Length;
                        byte[] leghtbytes = BitConverter.GetBytes(length);
                        NetworkTCP.SendTCP(leghtbytes, 4, stream);
                        NetworkTCP.SendTCP(bytes, length, stream);

                        goto tryagain;
                    }


                }

            }
            catch
            {
                Console.WriteLine("The client quit wihtout logging in..");

            }


            if (clients.Count() > 1) // If we have more than one client Broadcast the active users list 
            {

                ProtocolHandler.BroadcastActiveUsers(clients);

            }

           


            try
            {
                while (true) // Loop Reciever!!
                {
                    //-----------------------------------------------------------------
                    string message = ProtocolHandler.Recieve(stream);
                    //-----------------------------------------------------------------

                    //Console.WriteLine("{0}: {1}", username, message);

                    if (message == "####")
                    {
                        clients.Remove(currClientUsername);
                        Console.WriteLine("{0} has disconnected..", currClientUsername);
                        ProtocolHandler.BroadcastActiveUsers(clients);
                        break;
                    }
                    else if(message == "msgInComming")
                    {
                        
                        string toUsername = ProtocolHandler.Recieve(stream);
                        string sendMessage = ProtocolHandler.Recieve(stream);

                        Console.WriteLine(toUsername);
                        Console.WriteLine(sendMessage);

                        ProtocolHandler.Send(toUsername, "recieveMsg", clients);

                        ProtocolHandler.Send(toUsername, currClientUsername, clients);

                        ProtocolHandler.Send(toUsername, sendMessage, clients);


                    }
                    else if (message == "largeImageIncomming")
                    {

                        string toUsername = ProtocolHandler.Recieve(stream);
                        byte[] imgData = ProtocolHandler.ReceiveLargeImage(stream);

                        Console.WriteLine("Server recieved a image from {0} that should be sent to {1}", currClientUsername, toUsername);



                        /////////////////////////////////////////////////////////////////////////////////////////////


                        ProtocolHandler.Send(toUsername, "recieveAUUUBAUUU", clients);

                        ProtocolHandler.Send(toUsername, currClientUsername, clients);

                        Console.WriteLine("I am sending the image!");

                        ProtocolHandler.SendLargeImageFromBytes(toUsername, imgData, clients);

                        Console.WriteLine("Image Sent!!");


                    }else if(message== "emojiSend")
                    {
                        string toUsername = ProtocolHandler.Recieve(stream);

                        ProtocolHandler.Send(toUsername, "newEmoji", clients);

                        ProtocolHandler.Send(toUsername, currClientUsername, clients);

                        Console.WriteLine("Emoji Sent!!");

                    }
                    else
                    {

                    }

                }

            }
            catch
            {
                
                Console.WriteLine("{0} has disconnected..", currClientUsername);
                clients.Remove(currClientUsername);
                ProtocolHandler.BroadcastActiveUsers(clients);

            }
             
            client.Close();

        }
    }
}