using Microsoft.Extensions.Logging;
using System.Net;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// Author:     Seoin Kim and Gloria Shin
/// Partner:    Seoin Kim and Gloria Shin
/// Date:       06-Apr-2023
/// Course:     CS 3500, University of Utah, School of Computing
/// Copyright:  CS 3500, Gloria Shin, and Seoin Kim - This work may not 
/// be copied for use in Academic Courswork.
/// 
/// We, Seoin Kim and Gloria Shin, certify that we wrote this code from scratch and did not copy it in part or whole from another source. 
/// All references used in the completion of the assignments are cited in my README file.
/// 
/// File Contents
/// 
///     This contains the networking part of the Networking_and_Logging solution.
///     
/// </summary>

namespace Communications
{
    /// <summary>
    /// Represents a networking component that facilitates communication between clients and servers.
    /// </summary>
    public class Networking
    {
        // Delegates
        /// <summary>
        /// Represents a method that is called when a message arrives.
        /// </summary>
        public ReportMessageArrived _handleMessage;

        /// <summary>
        /// Represents a method that is called when a client disconnects.
        /// </summary>
        public ReportDisconnect _handleDisconnect;

        /// <summary>
        /// Represents a method that is called when a connection is established.
        /// </summary>
        public ReportConnectionEstablished _handleConnect;

        /// <summary>
        /// Gets or sets the underlying TCP client used by the networking component.
        /// </summary>
        public TcpClient _tcpClient { get; set; }

        /// <summary>
        /// A list of all clients currently connected.
        /// </summary>
        public List<TcpClient> clients = new();

        /// <summary>
        /// The termination character used to mark the end of a message.
        /// </summary>
        private readonly char _termCharacter;

        /// <summary>
        /// The cancellation token source used to cancel the "WaitFor" methods.
        /// </summary>
        private CancellationTokenSource _WaitForCancellation = new();

        /// <summary>
        /// The ID of the networking component.
        /// </summary>
        private string _id = string.Empty;

        /// <summary>
        /// The logger used by the networking component.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Gets or sets the ID of the networking component.
        /// </summary>
        public string ID
        {
            get
            {
                if (_id.Length == 0)
                {
                    _id = RemoteAddressPort;
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        /// <summary>
        /// Gets or sets the remote address and port of the TCP client.
        /// </summary>
        public string RemoteAddressPort
        {
            get
            {
                if (_tcpClient.Connected)
                {
                    return $"{_tcpClient.Client.RemoteEndPoint}";
                }
                else
                {
                    if (_tcpClient.Client != null)
                    {
                        return $"*{_tcpClient.Client.RemoteEndPoint}* - Disconnected";
                    }
                    else
                    {
                        return $"Client - Disconnected";
                    }
                }
            }
            set { }
        }


        /// <summary>
        /// Definition of what message was sent from what channel.
        /// </summary>
        /// <param name="channel"> the channel that the specified message </param>
        /// <param name="message"> the message sent from the channel </param>
        public delegate void ReportMessageArrived(Networking channel, string message);

        /// <summary>
        /// Definition of what channel wants to disconnect from the main server.
        /// </summary>
        /// <param name="channel"> the channel that wants to disconnect from the main server </param>
        public delegate void ReportDisconnect(Networking channel);

        /// <summary>
        /// Definition of what channel wants to connect to the main server. 
        /// </summary>
        /// <param name="channel"> the channel that wants to connect to the main server </param>
        public delegate void ReportConnectionEstablished(Networking channel);

        //public bool hasNewConnection = false;

        /// <summary>
        /// Builds a Networking element with an ILogger parameter, '\n', and delegates.
        /// </summary>
        /// <param name="logger"> the logging object provided by Dependency Injection from the main program </param>
        /// <param name="onConnect"> callback method </param>
        /// <param name="onDisconnect"> callback method </param>
        /// <param name="onMessage"> callback method </param>
        /// <param name="terminationCharacter"> the character defined to seperate one message from another </param>
        public Networking(ILogger logger, ReportConnectionEstablished onConnect, ReportDisconnect onDisconnect, ReportMessageArrived onMessage, char terminationCharacter)
        {
            _logger = logger;
            _handleConnect = onConnect;
            _handleDisconnect = onDisconnect;
            _handleMessage = onMessage;
            _termCharacter = terminationCharacter;
            _tcpClient = new TcpClient();
            _logger.LogInformation("A Networking project constructor was just made.");
        }

        /// <summary>0
        ///     Connects to a remote host at the specified
        ///     IP address and port number.
        /// </summary>
        /// <param name="host"> the IP address of the remote host to connect to </param>
        /// <param name="port"> the port number of the remote host to connect to </param>
        public void Connect(string host, int port)
        {

            try
            {
                // Check if the TcpClient is already connected. 
                if (_tcpClient.Connected) return;

                // If not, try to connect again.
                _tcpClient = new TcpClient(host, port);

                _logger.LogInformation($"Your id is: {ID} and the IP address is {_tcpClient.Client.RemoteEndPoint}");

                _handleConnect(this);
            }
            catch (Exception ex)
            {
                // If an exception occurs during the client process, log the error and disconnect.
                _logger.LogError($"Error connecting to remote host: {ex.Message}");
                _handleDisconnect(this);
            }
        }

        /// <summary>
        /// Waits for incoming client connections on the specified port. For each new client,
        /// creates a new Networking object and assigns the accepted TcpClient to it. 
        /// Calls the handleConnect callback with the new Networking object. 
        /// Starts a new thread to call the AwaitMessagesAsync method on the Networking object. 
        /// Logs the new client and adds it to the list of clients.
        /// </summary>
        /// <param name="port"> The port number to listen for connections on </param>
        /// <param name="infinite"> A boolean value indicating whether to keep listening for new connections </param>
        public async void WaitForClients(int port, bool infinite) // ChatServer (Listen, Receive, Read Message)
        {
            // Any ip address, port 11000
            TcpListener _tcpListener = new TcpListener(IPAddress.Any, port);

            // Start listening to a new client.
            _tcpListener.Start();

            _WaitForCancellation = new();

            try
            {
                while (infinite)
                {
                    // Create a new networking object and assign the accepted TcpClient to it.
                    Networking nw = new Networking(_logger, _handleConnect, _handleDisconnect, _handleMessage, '\n');
                    Networking nw2 = nw;
                    nw2._tcpClient = await _tcpListener.AcceptTcpClientAsync(_WaitForCancellation.Token); // pass in the cancellation token
                    nw._handleConnect(nw);

                    // Start a new thread to call the AwaitMessagesAsync method on the Networking object.
                    new Thread(() => nw.AwaitMessagesAsync(infinite)).Start();

                    // Add the new client to the list of clients.
                    lock (clients)
                    {
                        clients.Add(nw._tcpClient);
                    }

                    _logger.LogInformation($"\n ** New Connection ** Accepted From {nw2._tcpClient.Client.RemoteEndPoint} to {nw2._tcpClient.Client.LocalEndPoint}\n");
                }
            }
            catch (Exception)
            {
                _tcpListener.Stop();
                _logger.LogError("Error waiting for clients.");
            }
        }


        /// <summary>
        /// Asynchronously reads incoming messages from the network stream and calls the appropriate callback method.
        /// </summary>
        /// <param name="infinite"> If true, the method will run indefinitely until an exception is thrown. </param>
        public async void AwaitMessagesAsync(bool infinite = true) // ChatClient (Listen)
        {
            try
            {
                string parsedOutMessage;
                StringBuilder dataBacklog = new StringBuilder();
                byte[] buffer = new byte[4096];

                NetworkStream stream = _tcpClient.GetStream();

                if (stream == null) return;

                while (infinite)
                {
                    int total = await stream.ReadAsync(buffer);

                    if (total == 0)
                    {
                        throw new Exception();
                    }

                    string current_data = Encoding.UTF8.GetString(buffer, 0, total); // decode into string builder
                    current_data = current_data.Replace('\r', _termCharacter);
                    dataBacklog.Append(current_data);

                    _logger.LogInformation($" Received {total} new bytes for a total of {current_data.Length}.");

                    if (ParseMessageHandler(dataBacklog, out parsedOutMessage))
                    {
                        _logger.LogInformation($"This is the message: {parsedOutMessage}");
                        _handleMessage(this, parsedOutMessage);
                    }
                }
            }
            catch
            {
                _handleDisconnect(this);
            }
        }


        /// <summary>
        ///   Given a string (actually a string builder object)
        ///   check to see if it contains one or more messages as defined by
        ///   the termination character.
        /// </summary>
        /// 
        /// <param name="data"> all characters encountered so far </param>
        private bool ParseMessageHandler(StringBuilder data, out string message)
        {
            // Convert the StringBuilder object to a string.
            string allData = data.ToString();

            // Find the position of the termination character.
            int terminator_position = allData.IndexOf(_termCharacter);

            // If a termination character was found, extract the message and remove it from the data buffer.
            _logger.LogInformation($"This line is in ParseMessageHandler.");

            if (terminator_position >= 0)
            {
                message = allData.Substring(0, terminator_position);
                data.Remove(0, terminator_position + 1);

                return true;
            }

            // Otherwise, return false and an empty string.
            message = string.Empty;
            return false;
        }

        /// <summary>
        /// Stops waiting 
        /// </summary>
        public void StopWaitingForClients()
        {
            // Cancel the CancellationTokenSource to stop waiting for new clients.
            _WaitForCancellation.Cancel();
            _logger.LogInformation("You stopped waiting for clients.");
        }

        /// <summary>
        /// Disconnects the server.
        /// </summary>
        public void Disconnect()
        {
            _tcpClient.Close();
            _logger.LogInformation("You disconnected the server.");
        }

        /// <summary>
        /// Sends the specified text message to the connected client.
        /// </summary>
        /// <param name="text"> The message to send </param>
        public async void Send(TcpClient client, string text)
        {
            try
            {
                // Replace any occurrences of the terminator character with '\r' and append the terminator character.
                text = text.Replace(_termCharacter, '\r');

                // Convert the text to a byte array and send it to the client.
                byte[] data = Encoding.UTF8.GetBytes(text);


                // Use the network stream to synchronously write the data to the client.
                NetworkStream stream = client.GetStream();

                if (stream == null)
                {
                    // Log an error message if the client is not connected.
                    _logger.LogError($"Send Error: Client is not connected.");
                    return;
                }

                // await client get stream write async (data, data.length, 0);
                await stream.WriteAsync(data, 0, data.Length);

                // Log the sent message.
                _logger.LogInformation($"Sent message to client: '{text}'.");

            }
            catch (Exception ex)
            {
                // Call the onDisconnect callback if an exception is thrown.
                _handleDisconnect(this);
                _tcpClient.Close();

                // Log the exception.
                _logger.LogError($"Send Error: {ex.Message}");
            }
        }
    }
}