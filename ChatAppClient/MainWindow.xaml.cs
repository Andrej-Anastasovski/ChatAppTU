using ChatAppClient.Components; // To use the folder 
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ChatAppClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Connection connection = new Connection();

        public Dictionary<string, bool> currentNotifications = new Dictionary<string, bool>();


        //public static MainWindow windowInstance { get; private set; }


        public MainWindow()
        {

            InitializeComponent();
            connection.setWindowInstance(this);

        }

        // Stack Pannel Buttons ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void usernameSubmit_Click(object sender, RoutedEventArgs e)
        {
            // Username here
            if (inputBox.Text == string.Empty || inputPassBox.Password == string.Empty) // Clear InputBox.
            {

                badLogIn.Visibility = System.Windows.Visibility.Visible;
            }
            else
            {
                Regex usernameRegex = new Regex(@"^\w+(?:\s\w+)?$");// must be at least 3 characters long
                Regex passwordRegex = new Regex(@"^(?=.*\d).{8,}$");  // must contain at least one number and be at least 8 characters long

                String username = inputBox.Text; // Client adds the new username!
                String password = inputPassBox.Password;

                // Validate username and password
                if (usernameRegex.IsMatch(username) && passwordRegex.IsMatch(password))
                {

                    statusBarText.Text = "CHATTING APPLICATION";

                    ProtocolHandler.Send(username, this.connection.getConnectionClient());
                    ProtocolHandler.Send(password, this.connection.getConnectionClient());

                    profNameinGrid.Text = username;

                    startGrid.Visibility = System.Windows.Visibility.Collapsed;
                    mainGrid.Visibility = System.Windows.Visibility.Visible;

                }
                else
                {
                    inputBox.Text = string.Empty;
                    inputPassBox.Password = string.Empty;

                    badLogIn.Visibility = System.Windows.Visibility.Visible;
                }


            }
        }



        private void changeProfilePic_Click(object sender, RoutedEventArgs e)
        {

            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            bool? response = openFileDialog.ShowDialog();

            if (response == true)
            {

                string filepath = openFileDialog.FileName;


                ProtocolHandler.Send("profilePicChange", this.connection.getConnectionClient());


                ProtocolHandler.SendLargeImage(filepath, this.connection.getConnectionClient().GetStream());

                string fileName = Path.GetFileName(filepath);

                ProtocolHandler.Send(fileName, this.connection.getConnectionClient());


            }



        }


        // Main Interface Buttons ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btnHome_Click(object sender, RoutedEventArgs e)
        {
            chatRoom.Visibility = System.Windows.Visibility.Collapsed;
            settingsGrid.Visibility = System.Windows.Visibility.Collapsed;
            profileGrid.Visibility = System.Windows.Visibility.Collapsed;
        }


        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            chatRoom.Visibility = Visibility.Collapsed;
            profileGrid.Visibility = System.Windows.Visibility.Collapsed;

            settingsGrid.Visibility = System.Windows.Visibility.Visible;
        }


        private void btnProfile_Click(object sender, RoutedEventArgs e)
        {
            chatRoom.Visibility = Visibility.Collapsed;
            settingsGrid.Visibility = System.Windows.Visibility.Collapsed;

            profileGrid.Visibility = System.Windows.Visibility.Visible;
        }

        ///////////////////////////////////////////////////// Sign Up and Log In
        private void signUpbtn_Click(object sender, RoutedEventArgs e)
        {
            statusBarText.Text = "Sign Up";
            logInGrid.Visibility = System.Windows.Visibility.Collapsed;
            signUpGrid.Visibility = System.Windows.Visibility.Visible;
        }

        private void backToLogIn_Click(object sender, RoutedEventArgs e)
        {

            statusBarText.Text = "Log In";
            logInGrid.Visibility = System.Windows.Visibility.Visible;
            signUpGrid.Visibility = System.Windows.Visibility.Collapsed;

        }

        private void signUpSubmit_Click(object sender, RoutedEventArgs e)
        {


            String newUsername = inputNewusername.Text; // Client adds the new username!
            String newPassword = inputNewPassBox.Text;

            statusBarText.Text = "CHATTING APPLICATION";

            Regex usernameRegex = new Regex(@"^\w+(?:\s\w+)?$"); // must be at least 3 characters long
            Regex passwordRegex = new Regex(@"^(?=.*\d).{8,}$");  // must contain at least one number and be at least 8 characters long

            // Validate username and password
            if (usernameRegex.IsMatch(newUsername) && passwordRegex.IsMatch(newPassword))
            {
                Console.WriteLine("Username is valid.");



                ProtocolHandler.Send("reg" + newUsername, this.connection.getConnectionClient());
                ProtocolHandler.Send("reg" + newPassword, this.connection.getConnectionClient());

                signUpGrid.Visibility = System.Windows.Visibility.Collapsed;
                logInGrid.Visibility = System.Windows.Visibility.Visible;

                statusBarText.Text = "Log In";

            }
            else
            {
                inputNewusername.Text = string.Empty;
                inputNewPassBox.Text = string.Empty;

                badSignUpUsername.Visibility = System.Windows.Visibility.Visible;
                badSignUpPassword.Visibility = System.Windows.Visibility.Visible;
            }
        }

        // Window Bar Buttons ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private void btnMinimize_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }


        private void btnExit_Click(object sender, RoutedEventArgs e)
        {
            ProtocolHandler.Send("####", this.connection.getConnectionClient());
            Application.Current.Shutdown();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }


       

    }
}