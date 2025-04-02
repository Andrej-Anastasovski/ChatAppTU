# Chat Application

A robust client-server chat application built using C# with WPF for the client-side and TCP networking for the server-side. This application supports user authentication, real-time messaging, profile picture updates, and file sharing between users.

## Features

### Client-Side (WPF)
- **User Authentication**: Supports user sign-up and login with validation for username and password.
- **Real-Time Messaging**: Send and receive messages instantly between users.
- **Profile Management**: Change and display profile pictures dynamically.
- **User Interface**: Intuitive WPF-based interface with easy navigation between chat, profile, and settings.
- **File Sharing**: Send images and emojis directly through the chat interface.

### Server-Side (TCP Networking)
- **Client Management**: Manages multiple client connections with real-time broadcasting of active users.
- **User Authentication**: Handles user registration and login, storing credentials securely.
- **Messaging**: Supports real-time message delivery and file transfer between clients.
- **Database Integration**: Stores user credentials and other data using SQLite.

## Technologies Used
- **C#**
- **WPF (Windows Presentation Foundation)**
- **TCP Networking**
- **SQLite**
- **Dapper ORM**

## Usage

### Server
Run the server on your local machine or a remote server.

### Client
Launch the client application, sign up or log in, and start chatting with other connected users.
