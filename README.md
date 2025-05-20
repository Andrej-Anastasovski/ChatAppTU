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



# Chat Application Installation and Setup Guide

This guide provides step-by-step instructions for installing and setting up the Chat Application, which supports text messaging, file sharing, and user management.

## Prerequisites

To successfully run the Chat Application, ensure you have the following:

### Development Environment
- **Visual Studio 2022** (or later)
- **.NET 6.0 SDK** or newer
- **SQLite** support

## Installation Steps

### 1. Clone or Download the Repository
```bash
git clone [repository-url]
cd ChatApp
```
### 2. Database Setup
1. Ensure SQLite is installed on the server machine.
2. The database file should be located in the project directory.
3. If the database doesn't exist, create it with the following schema:
```sql
CREATE TABLE accounts (
    username TEXT PRIMARY KEY,
    password TEXT,
    profPic TEXT,
    status TEXT
);
```
4. Verify the connection string in the App.config file:
```xml
<connectionStrings>
    <add name="Default" connectionString="Data Source=.\ChatApp.db;Version=3;" providerName="System.Data.SqlClient" />
</connectionStrings>
```
### 3. Profile Pictures Setup
1. Create a folder named `profPictures` in the server project directory.
2. Add a default profile picture named `defaultProf.jpg` to this folder.

### 4. Building the Solution
1. Open the solution file (`ChatApp.sln`) in Visual Studio.
2. Restore NuGet packages:
   - Right-click on the solution in Solution Explorer.
   - Select "Restore NuGet Packages".
3. Build the solution:
   - Press `Ctrl+Shift+B` or select "Build > Build Solution".
   - Ensure there are no build errors.

### 5. Running the Application
The application consists of two components that need to be started separately:

#### Server Application
1. Right-click on the `ChatAppServer` project in Solution Explorer.
2. Select "Set as Startup Project".
3. Press `F5` or click the "Start" button.
4. The server console should display: `Server has started successfully on 127.0.0.1:10069`.

#### Client Application
1. After the server is running, right-click on the `ChatAppClient` project.
2. Select "Set as Startup Project".
3. Press `F5` or click the "Start" button.
4. The chat client interface should appear.

**Note**: You can run multiple client instances to test the chat functionality between users.

### 6. First-Time Usage
1. Use the Sign Up feature to create at least one account.
2. Log in using the created credentials.
3. To test multi-user functionality, launch another client instance and create/login with different credentials.

## Troubleshooting

### Connection Issues
- Ensure the server is running before starting any client.
- Verify firewall settings allow connections on port `10069`.
- Check that `127.0.0.1` is accessible.

### Database Issues
- Verify SQLite is properly installed.
- Check file permissions for the database file.
- Ensure the connection string is correct.

### Build Errors
- Make sure all NuGet packages are properly restored.
- Verify .NET SDK version is compatible.
- Check for any missing references in the solution.

## Running in Production

For production deployment:
1. Change the server IP from `127.0.0.1` to your server's public/network IP in `Connection.cs`.
2. Consider implementing a more secure authentication mechanism.
3. Ensure the database and profile picture directories have appropriate permissions.

