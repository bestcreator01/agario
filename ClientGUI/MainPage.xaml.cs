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

        ///// <summary>
        /////     the worlddrawable field.
        ///// </summary>
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
        ///     The timestamp of the last frame rendered in the game.
        /// </summary>
        public DateTime lastFrameTime;

        /// <summary>
        ///     The timer object used in this game duration.
        /// </summary>
        private System.Timers.Timer timer = new();

        /// <summary>
        ///     The clientPlayer object.
        /// </summary>
        Player clientPlayer = null;

        /// <summary>
        ///     The MainPage of ClientGUI.
        /// </summary>
        public MainPage(ILogger<MainPage> _logger)
        {
            InitializeComponent();
            worldDrawable = new(PlaySurface);
            logger = _logger;
            logger.LogInformation("This is a ChatClient MainPage.xaml.cs constructor.");
        }

        /// <summary>
        ///     Initializes the game logic.
        /// </summary>
        private void InitializeGameLogic()
        {
            initialized = true;

            // Assign your WorldDrawable to the PlaySurface.Drawable property.
            PlaySurface.Drawable = worldDrawable;

            //// Resize the widget.
            //PlaySurface.Width = 800;
            //Window.Height = 800;

            lastFrameTime = DateTime.Now;

            // The Timer should have a Tick event that calls a method GameStep.
            timer = new(2_000);
            timer.Elapsed += GameStep;
            timer.Start();
        }

        /// <summary>
        ///     This method is called repeatedly by a timer to update the game state 
        ///     and refresh the display.
        /// </summary>
        /// <param name="state"> An object containing state information for the timer (not used in this method). </param>
        /// <param name="args"> An object containing event data for the elapsed event (not used in this method). </param>
        void GameStep(object state, ElapsedEventArgs args)
        {
            // Tell the world model to AdvanceGameOneStep.`
            worldDrawable.gameObject.AdvanceGameOneStep();

            // Tell the play surface to redraw itself.
            PlaySurface.Invalidate();

            double fps = (DateTime.Now - lastFrameTime).TotalMilliseconds / 1000;
            lastFrameTime = DateTime.Now;

            // Update the position of the player.

            // Update the GUI labels to show the current location of the circle and its direction.
            Dispatcher.Dispatch(() =>
            {
                FPS.Text = $"FPS: {fps:F2}";

                // Get the client player object.
                if (worldDrawable.world.GetClientPlayer(out clientPlayer))
                {
                    CircleCenter.Text = $"Center: {clientPlayer.X:F2}, {clientPlayer.Y:F2}";
                    Direction.Text = $"Direction: {clientPlayer.Location:F2}";
                }
            });
        }

        /* Manage the GUI controls. */

        /// <summary>
        ///     Handles when the pointer is changed.
        /// </summary>
        /// <param name="sender"> The sender object </param>
        /// <param name="e"> The pointer event that is occurring </param>
        void PointerChanged(object sender, PointerEventArgs e)
        {
            Point? mousePosition = null;

            // Gets the mouse position.
            if (timer.Enabled && clientPlayer != null)
            {
                mousePosition = e.GetPosition(PlaySurface);

                // Get Player's X position.
                int mousePositionX = (int)clientPlayer.X + (int)mousePosition.Value.X - 300;

                // Get Player's Y position.
                int mousePositionY = (int)clientPlayer.Y + (int)mousePosition.Value.Y - 300;

                // Send Move request to the server.
                string message = String.Format(Protocols.CMD_Move, mousePositionX, mousePositionY);

                Match match = Regex.Match(message, Protocols.CMD_Move_Recognizer);
                bool matchesWithRecognizer = match.Success;

                if (matchesWithRecognizer)
                {
                    networking.Send(message);
                }

                // Redraw the circle.
                PlaySurface.Invalidate();
            }
        }

        /// <summary>
        ///     Handles when the spacebar is tapped.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> The tap event that is occuring </param>
        void OnTap(object sender, TappedEventArgs e)
        {
            //int floatToIntX = (int)ClientPlayer.X;
            //int floatToIntY = (int)ClientPlayer.Y;

            //string message = String.Format(Protocols.CMD_Split, floatToIntX, floatToIntY);

            // TODO - Only split when the player object's mass is 1000 or above

            // TODO - When splitting, make the player object * 2

            // TODO - Make the mass / 2

            //Match match = Regex.Match(message, Protocols.CMD_Split_Recognizer);
            //if (match.Success)
            //{
            //    networking.Send(networking._tcpClient, message);
            //}

            // TODO - Put CMD_Split and CMD_Split_Recognizer here.
        }

        /// <summary>
        ///     Handles when the mouse is dragged on the PlaySurface.
        ///     It helps you when zooming in and out.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> The pan event that is occuring </param>
        void PanUpdated(object sender, PanUpdatedEventArgs e)
        {
            // TODO
        }

        /// <summary>
        ///     Handles when the start game button is clicked. 
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

            // Start receiving messages from server.
            new Thread(() => networking.AwaitMessagesAsync(infinite: true)).Start();
            worldDrawable = new(PlaySurface);
            InitializeGameLogic();

            // Frontend (GUI) part
            Dispatcher.Dispatch(() =>
            {
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
            // Combine the ClientPlayer name with the CMD message.
            string playerName = EntryPlayerName.Text;
            string startGameMessage = string.Format(Protocols.CMD_Start_Game, playerName);

            // Check if it matches with Recognizer.
            Match match = Regex.Match(startGameMessage, Protocols.CMD_Start_Recognizer);
            bool matchesWithRecognizer = match.Success;

            // If the name matches with the recognizer, then send the name of a ClientPlayer to a server.
            if (matchesWithRecognizer)
            {
                channel.Send(startGameMessage + '\n');
                logger.LogInformation($"Message sent to server: startGameMessage");
            }
        }

        /// <summary>
        ///     Callback for when the connection to a network is lost.
        /// </summary>
        /// <param name="channel"> The networking channel where the disconnection occured. </param>
        void OnDisconnect(Networking channel)
        {
            // Send a message to the server that this client is disconnected.
            //networking.Disconnect();
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
                List<Food> foodList = JsonSerializer.Deserialize<List<Food>>(message.Substring(Protocols.CMD_Food.Length)) ?? throw new Exception("bad json");

                // Add the deserialized elements into the FoodList.
                foreach (var food in foodList)
                {
                    //world.FoodList.Add(food.ID, food);
                    worldDrawable.world.FoodList.Add(food.ID, food);
                }
            }
            if (message.StartsWith(Protocols.CMD_Player_Object))
            {
                if (worldDrawable.world.PlayerID == 0)
                {
                    // Obtain the player ID by deserializing the message from server.
                    long playerID = JsonSerializer.Deserialize<long>(message.Substring(Protocols.CMD_Player_Object.Length));

                    // Assign the value to the PlayerID in World.
                    worldDrawable.world.PlayerID = playerID;

                    worldDrawable.world.PlayerList.TryGetValue(playerID, out clientPlayer);
                }
            }
            if (message.StartsWith(Protocols.CMD_Dead_Players))
            {
                List<int> deadPlayers = JsonSerializer.Deserialize<List<int>>(message.Substring(Protocols.CMD_Dead_Players.Length));

                // Iterate over the ID of each dead ClientPlayer in deadPlayers.
                foreach (int deadPlayerID in deadPlayers)
                {
                    worldDrawable.world.PlayerList.Remove(deadPlayerID);
                }
            }
            if (message.StartsWith(Protocols.CMD_Eaten_Food))
            {
                List<long> eatenFoodsIDs = JsonSerializer.Deserialize<List<long>>(message.Substring(Protocols.CMD_Eaten_Food.Length));

                // Remove all eaten food objects in the food list
                foreach (var eatenFoodID in eatenFoodsIDs)
                {
                    if (worldDrawable.world.FoodList.ContainsKey(eatenFoodID))
                    {
                        worldDrawable.world.FoodList.Remove(eatenFoodID);
                    }
                }
            }
            if (message.StartsWith(Protocols.CMD_HeartBeat))
            {
                int heartBeat = JsonSerializer.Deserialize<int>(message.Substring(Protocols.CMD_HeartBeat.Length));
            }
            if (message.StartsWith(Protocols.CMD_Update_Players))
            {
                List<Player> updatePlayers = JsonSerializer.Deserialize<List<Player>>(message.Substring(Protocols.CMD_Update_Players.Length));

                // Iterate through the updated list of players.
                foreach (var player in updatePlayers)
                {
                    // If playerList contains the appropriate ClientPlayer ID, update ClientPlayer information.
                    if (worldDrawable.world.PlayerList.ContainsKey(player.ID))
                    {
                        // Update the information of existing players.
                        worldDrawable.world.PlayerList[player.ID] = player;

                    }
                    else
                    {
                        // Add a new player.
                        worldDrawable.world.PlayerList.Add(player.ID, player);
                    }
                }
            }
        }

    }
}