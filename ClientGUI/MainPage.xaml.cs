using System.Timers;
using System.Diagnostics;
using Communications;
using Microsoft.Extensions.Logging;
using AgarioModels;
using System.Text.Json;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

/// <summary>
/// Author:     Seoin Kim and Gloria Shin
/// Partner:    Seoin Kim and Gloria Shin
/// Date:       14-Apr-2023
/// Course:     CS 3500, University of Utah, School of Computing
/// Copyright:  CS 3500, Gloria Shin, and Seoin Kim - This work may not 
/// be copied for use in Academic Courswork.
/// 
/// We, Seoin Kim and Gloria Shin, certify that we wrote this code from scratch and did not copy it in part or whole from another source. 
/// All references used in the completion of the assignments are cited in my README file.
/// 
/// File Contents
/// 
///     This contains the codes to display the game on the client side.
///     
/// </summary>

namespace ClientGUI
{
    /// <summary>
    ///     The MainPage for ClientGUI.
    /// </summary>
    public partial class MainPage : ContentPage
    {
        /// <summary>
        ///     True if the GUI is initialized.
        /// </summary>
        public bool initialized = false;

        /// <summary>
        ///     The WorldDrawable field.
        /// </summary>
        public WorldDrawable worldDrawable;

        /// <summary>
        ///     The Networking field for a client to connect to a server.
        /// </summary>
        public Networking networking = null;

        /// <summary>
        ///     A logger object that we would use for debug purpose.
        /// </summary>
        private readonly ILogger logger;

        /// <summary>
        /// 
        /// </summary>
        public DateTime lastFrameTime;

        private Player player = null;

        /// <summary>
        ///     The MainPage of ClientGUI.
        /// </summary>
        public MainPage(ILogger<MainPage> _logger)
        {
            InitializeComponent();
            worldDrawable = new();
            logger = _logger;
        }

        /// <summary>
        ///    This method will be called every time the window is resized
        ///    including the first time the window "shows up" on the screen.
        /// </summary>
        /// <param name="width"> the width of the window </param>
        /// <param name="height"> the height of the window </param>
        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            Debug.WriteLine($"OnSizeAllocated {width} {height}");

            if (!initialized)
            {
                initialized = true;
                InitializeGameLogic();
            }
        }

        /// <summary>
        ///     Initializes the game logic.
        /// </summary>
        /// <exception cref="NotImplementedException"></exception>
        private void InitializeGameLogic()
        {
            // Assign your WorldDrawable to the PlaySurface.Drawable property.
            PlaySurface.Drawable = worldDrawable;

            // Resize the widget.
            Window.Width = 800;
            Window.Height = 800;

            lastFrameTime = DateTime.Now;
            // The Timer should have a Tick event that calls a method GameStep.
            System.Timers.Timer timer = new(2_000);
            timer.Elapsed += GameStep;
            timer.Start();
        }

        /// <summary>
        /// This method is called repeatedly by a timer to update the game state and refresh the display.
        /// </summary>
        /// <param name="state"> An object containing state information for the timer (not used in this method). </param>
        /// <param name="args"> An object containing event data for the elapsed event (not used in this method). </param>
        void GameStep(object state, ElapsedEventArgs args)
        {
            // Tell the world model to AdvanceGameOneStep.
            worldDrawable.gameObject.AdvanceGameOneStep();

            // Tell the play surface to redraw itself.
            PlaySurface.Invalidate();

            double fps = (DateTime.Now - lastFrameTime).TotalMilliseconds / 1000;
            lastFrameTime = DateTime.Now;

            // Update the GUI labels to show the current location of the circle and its direction.
            Dispatcher.Dispatch(() =>
            {
                FPS.Text = $"FPS: {fps:F2}";
                CircleCenter.Text = $"Center: {worldDrawable.gameObject.X:F2}, {worldDrawable.gameObject.Y:F2}";
                Direction.Text = $"Direction: {worldDrawable.gameObject.Location:F2}";
            });
        }

        // TODO - store/use the world model to store the game state.

        // TODO - handle communication with the server (our Networking class).

        // TODO - draw the game state.


        // Manage the GUI controls.

        /// <summary>
        /// TODO 
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        void StartGameButtonClicked(object sender, EventArgs e)
        {
            if (EntryPlayerName.Text == "")
            {
                Dispatcher.Dispatch(() =>
                {
                    Warning.IsVisible = true;
                    Warning.Text = "Please Type Your Name!";
                });
            }
            else
            {
                // Try connecting for the first time.
                if (networking == null)
                {
                    try
                    {
                        // Creates a new networking object to connect.
                        networking = new Networking(logger, OnConnect, OnDisconnect, OnMessage, '\n');

                        // Put the user name in the networking object.
                        networking.ID = EntryPlayerName.Text;

                        // Try connecting to server.
                        ConnectToServer();
                    }
                    catch (Exception ex)
                    {
                        ShowWarningMessage(ex);
                    }
                }
                else
                {
                    // Reconnecting..
                    try
                    {
                        ConnectToServer();
                    }
                    catch (Exception ex)
                    {
                        ShowWarningMessage(ex);
                    }
                }
            }
        }

        /// <summary>
        ///     Shows a warning message when the client fails to connect to the server.
        /// </summary>
        /// <param name="ex"> the error message </param>
        private void ShowWarningMessage(Exception ex)
        {
            // Frontend (GUI) part
            Dispatcher.Dispatch(() =>
            {
                Warning.IsVisible = true;
                Warning.Text = $"Failed to connect to the server: {ex.Message}";
            });
        }

        /// <summary>
        ///     A helper method that tries connecting to the server.
        /// </summary>
        private void ConnectToServer()
        {
            // Backend part
            networking.Connect(EntryServer.Text, 11000);
            networking.Send(networking._tcpClient, $"{networking.RemoteAddressPort}: Command Name {networking.ID}");
            networking.AwaitMessagesAsync();
            worldDrawable = new();
            player = new();

            // Frontend (GUI) part
            Dispatcher.Dispatch(() =>
            {
                // Update player name in the player class
                player.Name = EntryPlayerName.Text;

                Warning.IsVisible = false;
                StartScreen.IsVisible = false;
                GameScreen.IsVisible = true;
            });
        }

        /// <summary>
        ///     Callback for when the connection to a network is successful.
        /// </summary>
        /// <param name="channel"> The networking channel where the connection occured. </param>
        void OnConnect(Networking channel)
        {
            // Send a message to the server that this client is connected.
            channel.Send(channel._tcpClient, $"Client connected: {channel.ID}");
        }

        /// <summary>
        ///     Callback for when the connection to a network is lost.
        /// </summary>
        /// <param name="channel"> The networking channel where the disconnection occured. </param>
        void OnDisconnect(Networking channel)
        {
            // Send a message to the server that this client is disconnected.
        }

        /// <summary>
        ///     Callback for when a message arrives on the network.
        /// </summary>
        /// <param name="channel"> The networking channel where the message came from. </param>
        /// <param name="message"> The message that was received. </param>
        void OnMessage(Networking channel, string message)
        {
            if (message.StartsWith(Protocols.CMD_Food))
            {
                Food[] foods = JsonSerializer.Deserialize<Food[]>(message.Substring(Protocols.CMD_Food.Length)) ?? throw new Exception("bad json");
                
                // Add the deserialized elements into the FoodList.
                foreach (var food in foods)
                {
                    worldDrawable.world.FoodList.Add(food.ID, food);
                }

                // TODO - Add in GUI as well.
            }
            else if (message.StartsWith(Protocols.CMD_Player_Object))
            {
                long playerID = JsonSerializer.Deserialize<long>(message.Substring(Protocols.CMD_Player_Object.Length));
                
                // Add the player object into the PlayerList.
                worldDrawable.world.PlayerList.Add(playerID, player);
                player.ID = playerID;

                // TODO - Remove in GUI as well.
            }
            else if (message.StartsWith(Protocols.CMD_Dead_Players))
            {
                List<int> deadPlayers = JsonSerializer.Deserialize<List<int>>(message.Substring(Protocols.CMD_Dead_Players.Length));

                // Iterate over the ID of each dead player in deadPlayers.
                foreach (int deadPlayerID in deadPlayers)
                {
                    worldDrawable.world.PlayerList.Remove(deadPlayerID);
                }

                // TODO - Remove in GUI as well.
            }
            else if (message.StartsWith(Protocols.CMD_Eaten_Food))
            {
                List<long> eatenFoods = JsonSerializer.Deserialize<List<long>>(message.Substring(Protocols.CMD_Eaten_Food.Length));

                // Remove all eaten food objects in the food list
                foreach (var eatenFood in eatenFoods)
                {
                    if (worldDrawable.world.FoodList.ContainsKey(eatenFood))
                    {
                        worldDrawable.world.FoodList.Remove(eatenFood);
                    }
                }
                // TODO - Remove in GUI as well.
            }
            else if (message.StartsWith(Protocols.CMD_HeartBeat))
            {
                int heartBeat = JsonSerializer.Deserialize<int>(message.Substring(Protocols.CMD_HeartBeat.Length));

                // TODO - Use this in the information label.
            }
            else if (message.StartsWith(Protocols.CMD_Update_Players))
            {
                List<Player> updatePlayers = JsonSerializer.Deserialize<List<Player>>(message.Substring(Protocols.CMD_Update_Players.Length));

                // Iterate through the updated list of players.
                foreach (var player in updatePlayers)
                {
                    // If playerList contains the appropriate player ID, update player information.
                    if (worldDrawable.world.PlayerList.ContainsKey(player.ID))
                    {
                        var updatedPlayer = worldDrawable.world.PlayerList.TryGetValue(player.ID, out Player newplayer);
                        newplayer = player;
                    }
                }

                // TODO - use this in GUI as well.
            }
            else if (message.StartsWith(Protocols.CMD_Start_Game))
            {
                // TODO - ONLY SEND THIS AFTER THE CONNECTION HAS BEEN ESTABLISHED. (in onConnect)
                // AND AFTER THE PLAYER IS READY TO START PLAYING.
                
            }
            else if (message.StartsWith(Protocols.CMD_Start_Recognizer))
            {
                // Deserialize the regex expression
                string startRecognizer = JsonSerializer.Deserialize<string>(message.Substring(Protocols.CMD_Start_Recognizer.Length));
                string input = $"{{name,\"{EntryPlayerName.Text}\"}}";
                
                Match match = Regex.Match(input, startRecognizer);
                if (match.Success)
                {
                    channel.Send(channel._tcpClient, input);
                }
            }
            else if (message.StartsWith(Protocols.CMD_Move))
            {

            }
            else if (message.StartsWith(Protocols.CMD_Move_Recognizer))
            {

            }
            else if (message.StartsWith(Protocols.CMD_Split))
            {
                float[] splitCoordinates = JsonSerializer.Deserialize<float[]>(message.Substring(Protocols.CMD_Split.Length));
                float splitX = splitCoordinates[0];
                float splitY = splitCoordinates[1];

                // TODO - use these splitX and splitY when pressing spacebar to split toward.
            }
            else if (message.StartsWith(Protocols.CMD_Split_Recognizer))
            {

            }
        }
    }
}