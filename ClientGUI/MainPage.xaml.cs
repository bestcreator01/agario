﻿using System.Timers;
using Communications;
using Microsoft.Extensions.Logging;
using AgarioModels;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

/// <summary>
/// Author:     Seoin Kim and Gloria Shin
/// Partner:    Seoin Kim and Gloria Shin
/// Date:       17-Apr-2023
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
        ///     A field where it stores the value of heartbeat.
        /// </summary>
        private int heartbeat = 0;

        /// <summary>
        ///     The exact time when the game started.
        /// </summary>
        long unixStartTime = 0;

        /// <summary>
        ///     The MainPage of ClientGUI.
        /// </summary>
        public MainPage(ILogger<MainPage> _logger)
        {
            InitializeComponent();
            logger = _logger;

            worldDrawable = new(logger);
            logger.LogInformation("This is a ClientGUI MainPage.xaml.cs constructor.");
        }

        /// <summary>
        ///     Initializes the game logic.
        /// </summary>
        private void InitializeGameLogic()
        {
            // Assign your WorldDrawable to the PlaySurface.Drawable property.
            PlaySurface.Drawable = worldDrawable;

            lastFrameTime = DateTime.Now;

            // The Timer should have a Tick event that calls a method GameStep.
            timer = new(2_000);
            timer.Interval = 30;
            timer.Elapsed += GameStep;
            timer.Start();
            logger.LogInformation($"Game is initialized.");
        }

        /// <summary>
        ///     This method is called repeatedly by a timer to update the game state 
        ///     and refresh the display.
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
                Heartbeat.Text = $"Heartbeat: {heartbeat}";

                PlayerCount.Text = $"Amount of Player: {worldDrawable.world.PlayerList.Count()}";
                FoodCount.Text = $"Amount of Food: {worldDrawable.world.FoodList.Count()}";

                // Get the client player object.
                if (worldDrawable.world.GetClientPlayer())
                {
                    CircleCenter.Text = $"Center: {worldDrawable.world.ClientPlayer.X:F2}, {worldDrawable.world.ClientPlayer.Y:F2}";
                    Direction.Text = $"Direction: {worldDrawable.world.ClientPlayer.Location:F2}";
                    Mass.Text = $"Mass: {worldDrawable.world.ClientPlayer.Mass:F2}";
                }
            });

            logger.LogInformation($"Playsurface is being invalidated, as well as the game status.");
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
            if (timer.Enabled && worldDrawable.world.ClientPlayer != null)
            {
                mousePosition = e.GetPosition(PlaySurface);

                // Get Player's X position.
                int mousePositionX = (int)(mousePosition.Value.X * (worldDrawable.world.WorldWidth / worldDrawable.Width));

                // Get Player's Y position.
                int mousePositionY = (int)(mousePosition.Value.Y * (worldDrawable.world.WorldHeight / worldDrawable.Height));

                // Send Move request to the server.
                string message = String.Format(Protocols.CMD_Move, mousePositionX, mousePositionY);

                Match match = Regex.Match(message, Protocols.CMD_Move_Recognizer);
                bool matchesWithRecognizer = match.Success;

                if (matchesWithRecognizer)
                {
                    networking.Send(message);
                    logger.LogInformation($"The client player just changed their direction. Sent message to server: {message}");
                }
            }
        }

        /// <summary>
        ///     Handles when the left mouse is tapped.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        void OnTap(object sender, EventArgs e)
        {
            // Convert the coordinates from world to screen.
            string message = string.Format(Protocols.CMD_Split, (int)worldDrawable.screenClientPlayerX, (int)worldDrawable.screenClientPlayerY);

            Match match = Regex.Match(message, Protocols.CMD_Split_Recognizer);
            bool matchesWithRecognizer = match.Success;

            if (matchesWithRecognizer)
            {
                // Send split message
                networking.Send(message);
                logger.LogInformation($"Split button was clicked. Message sent to server: {message}");
            }
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
                logger.LogInformation($"The client failed to type their name.");
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

                        logger.LogInformation($"The client successfully connected to server.");
                    }
                    catch (Exception ex)
                    {
                        ShowWarningMessage(ex);
                        logger.LogError($"There was an error while connecting: {ex.Message}");
                    }
                }
                else
                {
                    // Reconnecting..
                    try
                    {
                        ConnectToServer();
                        logger.LogInformation($"Trying to reconnect to the server.");
                    }
                    catch (Exception ex)
                    {
                        ShowWarningMessage(ex);
                        logger.LogError($"There was an error while connecting: {ex.Message}");
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
            worldDrawable = new(logger);
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
        ///     Activates when the restart button is clicked.
        /// </summary>
        /// <param name="sender"> ignored </param>
        /// <param name="e"> ignored </param>
        void RestartButtonClicked(object sender, EventArgs e)
        {
            //worldDrawable.world.PlayerName = EntryPlayerName.Text;
            worldDrawable.world.ClientPlayer = null;

            networking.Connect(EntryServer.Text, 11000);

            // Start receiving messages from server.
            new Thread(() => networking.AwaitMessagesAsync(infinite: true)).Start();

            Dispatcher.Dispatch(() =>
            {
                Dead.IsVisible = false;
                Restart.IsVisible = false;
                deadImage.IsVisible = false;
            });

            logger.LogInformation($"The client requested to restart the game. The game is being restarted.");
        }

        /// <summary>
        ///     Callback for when the connection to a network is successful.
        /// </summary>
        /// <param name="channel"> The networking channel where the connection occured. </param>
        void OnConnect(Networking channel)
        {
            SendStartMessage(channel);
        }

        /// <summary>
        ///     Sends a start message to the server whenever the game starts.
        /// </summary>
        /// <param name="channel"> the networking object </param>
        private void SendStartMessage(Networking channel)
        {
            // Combine the ClientPlayer name with the CMD message.
            string startGameMessage = string.Format(Protocols.CMD_Start_Game, EntryPlayerName.Text);

            // Check if it matches with Recognizer.
            Match match = Regex.Match(startGameMessage, Protocols.CMD_Start_Recognizer);
            bool matchesWithRecognizer = match.Success;

            // If the name matches with the recognizer, then send the name of a ClientPlayer to a server.
            if (matchesWithRecognizer)
            {
                channel.Send(startGameMessage + '\n');
                logger.LogInformation($"The client just connected. Message sent to server: {startGameMessage}");
            }
        }

        /// <summary>
        ///     Callback for when the connection to a network is lost.
        /// </summary>
        /// <param name="channel"> The networking channel where the disconnection occured. </param>
        void OnDisconnect(Networking channel)
        {
            // Send a message to the server that this client is disconnected.
            logger.LogInformation($"The client just disconnected.");
        }

        /// <summary>
        ///     Callback for when a message arrives on the network.
        ///     
        ///     References: https://learnsql.com/blog/how-to-rank-rows-in-sql/
        ///                 https://stackoverflow.com/questions/17632584/how-to-get-the-unix-timestamp-in-c-sharp
        /// </summary>
        /// <param name="channel"> The networking channel where the message came from. </param>
        /// <param name="message"> The message that was received. </param>
        void OnMessage(Networking channel, string message)
        {
            logger.LogInformation($"Just received a message from the server: {message}");

            // Initialize the date for recording purposes
            if (message.StartsWith(Protocols.CMD_Food))
            {
                List<Food> foodList = JsonSerializer.Deserialize<List<Food>>(message.Substring(Protocols.CMD_Food.Length)) ?? throw new Exception("bad json");

                lock (worldDrawable.world.FoodList)
                {
                    // Add the deserialized elements into the FoodList.
                    foreach (var food in foodList)
                    {
                        if (worldDrawable.world.FoodList.ContainsKey(food.ID))
                        {
                            worldDrawable.world.FoodList[food.ID] = food;
                        }
                        else
                        {
                            worldDrawable.world.FoodList.Add(food.ID, food);
                        }
                    }
                }
            }
            if (message.StartsWith(Protocols.CMD_Player_Object))
            {
                // Obtain the player ID by deserializing the message from server.
                long playerID = JsonSerializer.Deserialize<long>(message.Substring(Protocols.CMD_Player_Object.Length));

                // Assign the value to the PlayerID in World.
                worldDrawable.world.PlayerID = playerID;

                // Update time
                unixStartTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();


                // Get the client player object.
                worldDrawable.world.GetClientPlayer();

                worldDrawable.world.ClientPlayer.Name = EntryPlayerName.Text;
            }
            if (message.StartsWith(Protocols.CMD_Dead_Players))
            {
                List<long> deadPlayersIDs = JsonSerializer.Deserialize<List<long>>(message.Substring(Protocols.CMD_Dead_Players.Length));

                lock (worldDrawable.world.PlayerList)
                {
                    // Iterate over the ID of each dead ClientPlayer in deadPlayers.
                    foreach (long deadPlayerID in deadPlayersIDs)
                    {
                        // If the player ID and one of the dead player's IDs are the same, show the restart button.
                        if (worldDrawable.world.PlayerID == deadPlayerID)
                        {
                            // Record ending time.                            
                            long unixEndTime = ((DateTimeOffset)DateTime.Now).ToUnixTimeSeconds();

                            // Insert data into the table based on the player/game status.
                            using (SqlConnection connection = new SqlConnection(WebServer.WebServer.connectionString))
                            {
                                connection.Open();
                                using (SqlCommand command = new SqlCommand("DECLARE @gameID INT; " +
                                    "INSERT INTO Game (playerID, playerName, endTime) VALUES (@ID, @playerName, @endTime);" +
                                    "SET @gameID = SCOPE_IDENTITY();" +
                                    "INSERT INTO Player (playerID, playerName) VALUES (@ID, @playerName);" +
                                    "INSERT INTO Time (gameID, playerID, startTime) VALUES (@gameID, @ID, @startTime);" +
                                    "INSERT INTO Mass (gameID, playerID, Mass) VALUES (@gameID, @ID, @mass);" +
                                    "INSERT INTO Heartbeat (playerID, heartbeat) VALUES (@ID, @duration);" +
                                    "INSERT INTO LeaderBoard (gameID, playerID, playerName, Rank, Mass)" +
                                    "SELECT " +
                                        "@gameID as gameID, " +
                                        "@ID as playerID, " +
                                        "@playerName as playerName, " +
                                        "RANK() OVER(ORDER BY MAX(m.Mass) DESC) as Rank, " +
                                        "MAX(Mass) AS Mass " +
                                    "FROM Game g " +
                                    "JOIN Player p ON g.playerID = p.playerID AND g.playerName = p.playerName " +
                                    "JOIN Mass m ON g.gameID = m.gameID AND p.playerID = m.playerID " +
                                    "WHERE p.playerName = @playerName " +
                                    "GROUP BY p.playerName;", connection))
                                {
                                    // Set the parameter values and execute the query
                                    command.Parameters.AddWithValue("@playerName", worldDrawable.world.ClientPlayer.Name);
                                    command.Parameters.AddWithValue("@ID", worldDrawable.world.PlayerID);
                                    command.Parameters.AddWithValue("@startTime", unixStartTime);
                                    command.Parameters.AddWithValue("@endTime", unixEndTime);
                                    command.Parameters.AddWithValue("@mass", worldDrawable.world.ClientPlayer.Mass);
                                    command.Parameters.AddWithValue("@duration", heartbeat);

                                    command.ExecuteNonQuery();

                                    // Update rank based on new incoming data.
                                    using (SqlCommand updateRankCommand = new SqlCommand("UPDATE LeaderBoard " +
                                        "SET Rank = subquery.Rank " +
                                        "FROM LeaderBoard " +
                                        "JOIN ( " +
                                        "   SELECT p.playerID, RANK() OVER (ORDER BY MAX(m.Mass) DESC) AS Rank " +
                                        "   FROM Game g " +
                                        "   JOIN Player p ON g.playerID = p.playerID " +
                                        "   JOIN Mass m ON g.gameID = m.gameID AND p.playerID = m.playerID " +
                                        "   GROUP BY p.playerID " +
                                        ") AS subquery ON LeaderBoard.playerID = subquery.playerID", connection))
                                    {
                                        updateRankCommand.ExecuteNonQuery();
                                    }
                                }
                            }

                            Dispatcher.Dispatch(() =>
                            {
                                Dead.IsVisible = true;
                                Dead.Text = "HAHA YOU ARE DEAD!";
                                Restart.IsVisible = true;
                                deadImage.RelScaleTo(0.5);
                                deadImage.IsVisible = true;
                            });

                        }

                        // Remove the IDs of dead players.
                        if (worldDrawable.world.PlayerList.ContainsKey(deadPlayerID))
                        {
                            worldDrawable.world.PlayerList.Remove(deadPlayerID);
                            networking.Disconnect();
                        }
                    }
                }
            }
            if (message.StartsWith(Protocols.CMD_Eaten_Food))
            {
                List<long> eatenFoodsIDs = JsonSerializer.Deserialize<List<long>>(message.Substring(Protocols.CMD_Eaten_Food.Length));

                lock (worldDrawable.world.FoodList)
                {
                    // Remove all eaten food objects in the food list
                    foreach (var eatenFoodID in eatenFoodsIDs)
                    {
                        if (worldDrawable.world.FoodList.ContainsKey(eatenFoodID))
                        {
                            worldDrawable.world.FoodList.Remove(eatenFoodID);
                        }
                    }
                }
            }
            if (message.StartsWith(Protocols.CMD_HeartBeat))
            {
                heartbeat = JsonSerializer.Deserialize<int>(message.Substring(Protocols.CMD_HeartBeat.Length));
            }
            if (message.StartsWith(Protocols.CMD_Update_Players))
            {
                List<Player> updatePlayers = JsonSerializer.Deserialize<List<Player>>(message.Substring(Protocols.CMD_Update_Players.Length));

                lock (worldDrawable.world.PlayerList)
                {
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
}
