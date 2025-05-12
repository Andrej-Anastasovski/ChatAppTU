using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChatAppClient.Components
{
    public class Connection
    {
        private TcpClient client;
        Int32 port = 10069;
        IPAddress serverAddress = IPAddress.Parse("127.0.0.1");

        private MainWindow mainWindow;

        private Thread reciever;


        public Dictionary<string, StackPanel> activeUserMsgPanel = new Dictionary<string, StackPanel>();


        public Connection()
        {
            client = new TcpClient();
            client.Connect(serverAddress, port);
            NetworkStream stream = client.GetStream();
            reciever = new Thread(Start);
            reciever.IsBackground = true;
            reciever.Start();
        }

        // State machines 
        // Draw Rectangular Boxes 
        // Separotor 


        public void Start()
        {

            NetworkStream stream = client.GetStream();

            while (true)
            {
                //--------------------------------------------------------------------Recieve!
                byte[] sizeBytes = NetworkTCP.ReceiveTCP(4, stream);
                int messageSize = BitConverter.ToInt32(sizeBytes);
                byte[] messageBytes = NetworkTCP.ReceiveTCP(messageSize, stream);
                string response = Encoding.ASCII.GetString(messageBytes);
                //--------------------------------------------------------------------


                Debug.WriteLine(response);

                if (response.Substring(0, 4) == "!@#$")
                {
                    response = response.Remove(0, 4);
                    string username = response;
                    string status = ProtocolHandler.Recieve(stream);
                    byte[] imgData = ProtocolHandler.ReceiveLargeImage(stream);
                    ImageSource? bitmapImage = BinaryToImageSource(imgData);



                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        bool showNotification = mainWindow.currentNotifications.TryGetValue(username, out bool isNotificationEnabled) && isNotificationEnabled;
                        ActiveUser newActiveUser = new ActiveUser(username, status, bitmapImage, this, showNotification);
                        mainWindow.stackPanel.Children.Add(newActiveUser);
                    });



                }
                else if (response == "resetThePannel!")
                {
                    mainWindow.Dispatcher.Invoke(() =>
                    {
                        mainWindow.stackPanel.Children.Clear();
                    });
                }
                else if (response == "profPicset")
                {
                    Debug.WriteLine("About to recieve an image");
                    byte[] imgData = ProtocolHandler.ReceiveLargeImage(stream);
                    // byte[] imgData = recieveImgByteArr(stream);
                    ImageSource? bitmapImage = BinaryToImageSource(imgData);

                    mainWindow.Dispatcher.Invoke(() =>
                    {

                        mainWindow.urProfPic.ImageSource = bitmapImage;
                        mainWindow.profGridprofPic.Source = bitmapImage;

                    });
                }
                else if (response == "recieveMsg")
                {
                    string fromuser = ProtocolHandler.Recieve(stream);
                    string recievedMessage = ProtocolHandler.Recieve(stream);

                    mainWindow.Dispatcher.Invoke(() =>
                    {

                        if (!activeUserMsgPanel.ContainsKey(fromuser))
                        {
                            activeUserMsgPanel[fromuser] = new StackPanel();
                        }
                        activeUserMsgPanel[fromuser].Children.Add(new Message(recievedMessage, "left"));

                        /////////////////////////////////////////////// NOTIFICATION HEEEEREEE!!!!
                        ///

                        // Find the ActiveUser control that corresponds to the sender
                        var activeUserControl = mainWindow.stackPanel.Children
                            .OfType<ActiveUser>()
                            .FirstOrDefault(c => c.aUserName == fromuser);

                        if (activeUserControl != null)
                        {
                            // Show the notification on the ActiveUser control
                            activeUserControl.ShowNotification();
                        }

                        mainWindow.currentNotifications[fromuser] = true;

                    });


                }
                else if (response == "recieveAUUUBAUUU")
                {

                    string fromuser = ProtocolHandler.Recieve(stream);


                    byte[] imgData = ProtocolHandler.ReceiveLargeImage(stream);


                    BitmapImage bitmapImage = CreateBitmapFromBytes(imgData);

                    mainWindow.Dispatcher.Invoke(() =>
                    {

                        if (!activeUserMsgPanel.ContainsKey(fromuser))
                        {
                            activeUserMsgPanel[fromuser] = new StackPanel();
                        }

                        try
                        {
                            activeUserMsgPanel[fromuser].Children.Add(new ImageMessage(bitmapImage, "left"));
                        }
                        catch
                        {
                            activeUserMsgPanel[fromuser].Children.Add(new Message("ERROR Image could not be dispalyed!", "left"));
                        }

                        var activeUserControl = mainWindow.stackPanel.Children
                            .OfType<ActiveUser>()
                           .FirstOrDefault(c => c.aUserName == fromuser);

                        if (activeUserControl != null)
                        {
                            // Show the notification on the ActiveUser control
                            activeUserControl.ShowNotification();
                        }
                        mainWindow.currentNotifications[fromuser] = true;

                    });


                }
                else if (response == "newEmoji")
                {
                    string fromuser = ProtocolHandler.Recieve(stream);

                    BitmapImage hehe = new BitmapImage(new Uri(@"\Visuals\emoji.png", UriKind.Relative));


                    mainWindow.Dispatcher.Invoke(() =>
                    {

                        if (!activeUserMsgPanel.ContainsKey(fromuser))
                        {
                            activeUserMsgPanel[fromuser] = new StackPanel();
                        }

                        try
                        {
                            activeUserMsgPanel[fromuser].Children.Add(new ImageMessage(hehe, "left"));
                        }
                        catch
                        {
                            activeUserMsgPanel[fromuser].Children.Add(new Message("ERROR Image could not be dispalyed!", "left"));
                        }

                        var activeUserControl = mainWindow.stackPanel.Children
                           .OfType<ActiveUser>()
                           .FirstOrDefault(c => c.aUserName == fromuser);

                        if (activeUserControl != null)
                        {
                            // Show the notification on the ActiveUser control
                            activeUserControl.ShowNotification();
                        }

                        mainWindow.currentNotifications[fromuser] = true;

                    });


                }
                else if (response == "FailedToSignIn")
                {


                    mainWindow.Dispatcher.Invoke(() =>
                    {

                        mainWindow.startGrid.Visibility = System.Windows.Visibility.Visible;
                        mainWindow.mainGrid.Visibility = System.Windows.Visibility.Collapsed;
                        mainWindow.badLogIn.Visibility = System.Windows.Visibility.Visible;

                        mainWindow.inputBox.Text = string.Empty;
                        mainWindow.inputPassBox.Password = string.Empty;


                    });


                }
                else
                {



                }
            }
        }


        public void setWindowInstance(MainWindow mainWindow)
        {
            this.mainWindow = mainWindow;
        }


        private ImageSource? BinaryToImageSource(byte[] imageBinary)
        {
            return (ImageSource?)new ImageSourceConverter().ConvertFrom(imageBinary);
        }

        public TcpClient getConnectionClient()
        {

            return this.client;

        }

        public BitmapImage CreateBitmapFromBytes(byte[] imgData)
        {
            BitmapImage bitmap = new BitmapImage();

            using (MemoryStream stream = new MemoryStream(imgData))
            {
                bitmap.BeginInit();
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            bitmap.Freeze();

            return bitmap;
        }
    }
}
