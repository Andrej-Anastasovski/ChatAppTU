using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChatAppClient.Components
{
    public class Connection
    {
        private TcpClient client;
        private Thread receiver;
        private MainWindow mainWindow;
        public Dictionary<string, StackPanel> activeUserMsgPanel = new Dictionary<string, StackPanel>();

        public Connection()
        {
            client = new TcpClient();
            client.Connect("127.0.0.1", 10069); // Connect to the server
            receiver = new Thread(Start);
            receiver.IsBackground = true;
            receiver.Start();
        }

        public void Start()
        {
            NetworkStream stream = client.GetStream();

            while (true)
            {
                byte[] sizeBytes = NetworkTCP.ReceiveTCP(4, stream);
                int messageSize = BitConverter.ToInt32(sizeBytes);
                byte[] messageBytes = NetworkTCP.ReceiveTCP(messageSize, stream);
                string response = Encoding.ASCII.GetString(messageBytes);

                if (response.Substring(0, 4) == "!@#$") // Handle username, status, and image
                {
                    ProcessUserInfo(response, stream);
                }
                else if (response == "resetThePannel!") // Clear the user panel
                {
                    ResetPanel();
                }
                else if (response == "profPicset") // Handle profile picture update
                {
                    UpdateProfilePicture(stream);
                }
                else if (response == "recieveMsg") // Handle incoming message
                {
                    ReceiveMessage(stream);
                }
                else if (response == "recieveAUUUBAUUU") // Handle incoming image
                {
                    ReceiveImage(stream);
                }
                else if (response == "newEmoji") // Handle incoming emoji
                {
                    ReceiveEmoji(stream);
                }
                else if (response == "FailedToSignIn") // Handle failed sign-in
                {
                    HandleFailedSignIn();
                }
            }
        }

        private void ProcessUserInfo(string response, NetworkStream stream)
        {
            response = response.Remove(0, 4); // Remove header
            string username = response;
            string status = ProtocolHandler.Recieve(stream);
            byte[] imgData = ProtocolHandler.ReceiveLargeImage(stream);
            ImageSource? bitmapImage = BinaryToImageSource(imgData);

            mainWindow.Dispatcher.Invoke(() =>
            {
                ActiveUser newActiveUser = new ActiveUser(username, status, bitmapImage, this, true);
                mainWindow.stackPanel.Children.Add(newActiveUser);
            });
        }

        private void ResetPanel()
        {
            mainWindow.Dispatcher.Invoke(() =>
            {
                mainWindow.stackPanel.Children.Clear();
            });
        }

        private void UpdateProfilePicture(NetworkStream stream)
        {
            byte[] imgData = ProtocolHandler.ReceiveLargeImage(stream);
            ImageSource? bitmapImage = BinaryToImageSource(imgData);
            mainWindow.Dispatcher.Invoke(() =>
            {
                mainWindow.urProfPic.ImageSource = bitmapImage;
                mainWindow.profGridprofPic.Source = bitmapImage;
            });
        }

        private void ReceiveMessage(NetworkStream stream)
        {
            string fromUser = ProtocolHandler.Recieve(stream);
            string receivedMessage = ProtocolHandler.Recieve(stream);

            mainWindow.Dispatcher.Invoke(() =>
            {
                if (!activeUserMsgPanel.ContainsKey(fromUser))
                {
                    activeUserMsgPanel[fromUser] = new StackPanel();
                }
                activeUserMsgPanel[fromUser].Children.Add(new Message(receivedMessage, "left"));
                ShowNotification(fromUser);
            });
        }

        private void ReceiveImage(NetworkStream stream)
        {
            string fromUser = ProtocolHandler.Recieve(stream);
            byte[] imgData = ProtocolHandler.ReceiveLargeImage(stream);
            BitmapImage bitmapImage = CreateBitmapFromBytes(imgData);

            mainWindow.Dispatcher.Invoke(() =>
            {
                if (!activeUserMsgPanel.ContainsKey(fromUser))
                {
                    activeUserMsgPanel[fromUser] = new StackPanel();
                }
                activeUserMsgPanel[fromUser].Children.Add(new ImageMessage(bitmapImage, "left"));
                ShowNotification(fromUser);
            });
        }

        private void ReceiveEmoji(NetworkStream stream)
        {
            string fromUser = ProtocolHandler.Recieve(stream);
            BitmapImage emojiImage = new BitmapImage(new Uri(@"\Visuals\emoji.png", UriKind.Relative));

            mainWindow.Dispatcher.Invoke(() =>
            {
                if (!activeUserMsgPanel.ContainsKey(fromUser))
                {
                    activeUserMsgPanel[fromUser] = new StackPanel();
                }
                activeUserMsgPanel[fromUser].Children.Add(new ImageMessage(emojiImage, "left"));
                ShowNotification(fromUser);
            });
        }

        private void HandleFailedSignIn()
        {
            mainWindow.Dispatcher.Invoke(() =>
            {
                mainWindow.startGrid.Visibility = System.Windows.Visibility.Visible;
                mainWindow.mainGrid.Visibility = System.Windows.Visibility.Collapsed;
                mainWindow.badLogIn.Visibility = System.Windows.Visibility.Visible;
            });
        }

        private void ShowNotification(string fromUser)
        {
            var activeUserControl = mainWindow.stackPanel.Children
                .OfType<ActiveUser>()
                .FirstOrDefault(c => c.aUserName == fromUser);

            if (activeUserControl != null)
            {
                activeUserControl.ShowNotification();
            }

            mainWindow.currentNotifications[fromUser] = true;
        }

        private ImageSource? BinaryToImageSource(byte[] imageBinary)
        {
            return (ImageSource?)new ImageSourceConverter().ConvertFrom(imageBinary);
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

