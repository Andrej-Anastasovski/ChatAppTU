using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ChatAppClient.Components
{
    internal class ActiveUser : Border
    {

        public MainWindow mainWindow = Application.Current.MainWindow as MainWindow;
        public string aUserName { get; set; }
        public string Status { get; set; }
        public Connection connection { get; set; }
        public ImageSource source { get; set; }

        public NetworkStream stream { get; set; }

        public ActiveUser(string name, string status, ImageSource s, Connection conn, bool notSwitch) : base()
        {
            aUserName = name;
            Status = status;
            source = s;
            connection = conn;
            stream = conn.getConnectionClient().GetStream();


            Name = name;
            CornerRadius = new CornerRadius(7);
            BorderBrush = new SolidColorBrush(Colors.Blue);
            BorderThickness = new Thickness(0.5);
            Background = new SolidColorBrush(Colors.Black);

            MouseEnter += ActiveUser_MouseEnter;
            MouseLeave += ActiveUser_MouseLeave;
            PreviewMouseLeftButtonDown += ActiveUser_PreviewMouseLeftButtonDown;



            Grid grid = new Grid();
            grid.Name = Name;
            grid.Width = 276;
            grid.Height = 58;

            Border innerBorder = new Border();
            innerBorder.Name = Name + "border"; // Used to find the specific item in stack pannel!
            innerBorder.Margin = new Thickness(5, 5, 219, 5);
            innerBorder.CornerRadius = new CornerRadius(50);
            innerBorder.BorderBrush = new SolidColorBrush(Colors.Red);


            Ellipse notification = new Ellipse()
            {
                Fill = new SolidColorBrush(Colors.Red),
                Width = 10,
                Height = 10,
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
                Margin = new Thickness(0, 5, 5, 0),
                Visibility = Visibility.Collapsed // Initially hidden

            };

            if (notSwitch == true)
            {
                notification.Visibility = Visibility.Visible;
            }


            ImageBrush imageBrush = new ImageBrush();
            imageBrush.ImageSource = source;
            //
            innerBorder.Background = imageBrush;

            TextBlock nameBlock = new TextBlock();
            nameBlock.Margin = new Thickness(69, 19, 10, 19);
            nameBlock.Text = name;

            TextBlock statusBlock = new TextBlock();
            statusBlock.Margin = new Thickness(69, 39, 10, 10);
            statusBlock.Text = "Status: " + status;
            statusBlock.FontSize = 9;

            // assemble the blocks
            grid.Children.Add(notification);


            grid.Children.Add(innerBorder);
            grid.Children.Add(nameBlock);
            grid.Children.Add(statusBlock);
            Child = grid;


        }

        public void ShowNotification()
        {
            var grid = this.Child as Grid;
            if (grid != null)
            {
                var notificationEllipse = grid.Children
                    .OfType<Ellipse>()
                    .FirstOrDefault();

                if (notificationEllipse != null)
                {
                    notificationEllipse.Visibility = Visibility.Visible;
                }
            }
        }


        public void HideNotification()
        {
            var grid = this.Child as Grid;
            if (grid != null)
            {
                var notificationEllipse = grid.Children
                    .OfType<Ellipse>()
                    .FirstOrDefault();

                if (notificationEllipse != null)
                {
                    notificationEllipse.Visibility = Visibility.Collapsed;
                }
            }
        }




        private void ActiveUser_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) // Active Users, When Clicked on!
        {

            AddChatRoom(mainWindow.gridThatContainsTextInput);

            mainWindow.msgStackPanel.Children.Clear();

            if (connection.activeUserMsgPanel.ContainsKey(Name))
            {

                mainWindow.msgStackPanel.Children.Add(connection.activeUserMsgPanel[Name]);

            }
            else
            {
                connection.activeUserMsgPanel[Name] = new StackPanel();
                mainWindow.msgStackPanel.Children.Add(connection.activeUserMsgPanel[Name]);
            }

            var activeUserControl = mainWindow.stackPanel.Children
                 .OfType<ActiveUser>()
                 .FirstOrDefault(c => c.aUserName == Name);

            if (activeUserControl != null)
            {
                // Show the notification on the ActiveUser control
                activeUserControl.HideNotification();
            }

            mainWindow.currentNotifications[Name] = false;

        }



        private void ActiveUser_MouseEnter(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Change background color to highlight color when mouse enters

            if (Status == "Active")
            {
                Background = new SolidColorBrush(Colors.LimeGreen);
            }
            else
            {
                Background = new SolidColorBrush(Colors.PaleVioletRed);
            }

        }



        private void ActiveUser_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            // Change background color back to transparent when mouse leaves
            Background = new SolidColorBrush(Colors.Black);
        }


        private void AddChatRoom(Grid parentContainer)
        {

            mainWindow.chatRoom.Visibility = System.Windows.Visibility.Visible;

            mainWindow.chatRoomName.Text = Name;
            mainWindow.progPicOfActiveUser.ImageSource = source;


            // Create a new instance of the Grid
            Grid chatRoom = new Grid();

            // Create and configure the Button element
            Button btnSendMsg = new Button()
            {
                Content = new StackPanel()
                {
                    Children =
            {
                new Image()
                {
                    Source = new BitmapImage(new Uri(@"\Visuals\sendMSGIcon.png", UriKind.Relative)),
                    Height = 25,
                    Width = 31
                }
            }
                },
                Name = "btnSendMsg",
                FontSize = 16,
                FontFamily = new FontFamily("Montserrat"),
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 73, 115)),
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand,
                Margin = new Thickness(549, 0, 0, -1)
            };

            Button btnBrowseImage = new Button()
            {
                Content = new StackPanel()
                {
                    Children =
        {
            new Image()
            {
                Source = new BitmapImage(new Uri(@"\Visuals\attachImage.png", UriKind.Relative)),
                Height = 25,
                Width = 31
            }
        }
                },
                Name = "btnBrowseImage",
                FontSize = 16,
                FontFamily = new FontFamily("Montserrat"),
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 73, 115)),
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand,
                Margin = new Thickness(54, 0, 490, 0)
            };

            Button btnBrowseEmoji = new Button()
            {
                Content = new StackPanel()
                {
                    Children =
        {
            new Image()
            {
                Source = new BitmapImage(new Uri(@"\Visuals\iconForEmoji.png", UriKind.Relative)),
                Height = 25,
                Width = 31
            }
        }
                },
                Name = "btnBrowseEmoji",
                FontSize = 16,
                FontFamily = new FontFamily("Montserat"),
                FontWeight = FontWeights.Bold,
                Foreground = Brushes.White,
                Background = Brushes.Transparent,
                BorderBrush = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(28, 73, 115)),
                BorderThickness = new Thickness(1),
                Cursor = Cursors.Hand,
                Margin = new Thickness(0, 0, 549, 0)
            };





            // Subscribe to the Click event of the Button
            btnSendMsg.Click += btnSendMsg_Click;

            btnBrowseImage.Click += btnBrowseImage_Click;

            btnBrowseEmoji.Click += BtnBrowseEmoji_Click;

            // Add the Button to the Grid
            chatRoom.Children.Add(btnSendMsg);
            chatRoom.Children.Add(btnBrowseEmoji);
            chatRoom.Children.Add(btnBrowseImage);

            // Add the Grid to the parent container
            parentContainer.Children.Add(chatRoom);
        }

        private void BtnBrowseEmoji_Click(object sender, RoutedEventArgs e)
        {

            ProtocolHandler.Send("emojiSend", this.connection.getConnectionClient());

            ProtocolHandler.Send(Name, this.connection.getConnectionClient());

            BitmapImage bitmapImage = new BitmapImage(new Uri(@"\Visuals\emoji.png", UriKind.Relative));

            connection.activeUserMsgPanel[Name].Children.Add(new ImageMessage(bitmapImage, "right"));

        }

        private void btnSendMsg_Click(object sender, RoutedEventArgs e)
        {

            // Find the sendMessageInput TextBox by name
            TextBox sendMessageInput = mainWindow.sendMessageInput;



            string message = sendMessageInput.Text;

            if (message != "")
            {
                ProtocolHandler.Send("msgInComming", this.connection.getConnectionClient());

                ProtocolHandler.Send(Name, this.connection.getConnectionClient());

                ProtocolHandler.Send(message, this.connection.getConnectionClient());


                connection.activeUserMsgPanel[Name].Children.Add(new Message(message, "right"));


                mainWindow.sendMessageInput.Text = "";
            }
            else
            {
                ProtocolHandler.Send("Trying to send an empty message!", this.connection.getConnectionClient());
            }


        }


        private void btnBrowseImage_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            bool? response = openFileDialog.ShowDialog();

            if (response == true)
            {

                string filepath = openFileDialog.FileName;


                ProtocolHandler.Send("largeImageIncomming", this.connection.getConnectionClient());

                ProtocolHandler.Send(Name, this.connection.getConnectionClient());
                ProtocolHandler.SendLargeImage(filepath, stream);


                BitmapImage bitmap = new BitmapImage(new Uri(filepath));

                connection.activeUserMsgPanel[Name].Children.Add(new ImageMessage(bitmap, "right"));

            }
        }
    }
}