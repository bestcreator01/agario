using System.Timers;
using System.Diagnostics;
using Communications;
using Microsoft.Extensions.Logging;
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
            worldDrawable = new();

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
        }
    }
}