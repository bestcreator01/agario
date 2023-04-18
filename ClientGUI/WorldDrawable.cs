using AgarioModels;
using Microsoft.Extensions.Logging;

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
///     This class contains the code for drawing the world and game objects.
///     
/// </summary>
namespace ClientGUI
{
    /// <summary>
    ///     This class represents a drawable world.
    /// </summary>
    public class WorldDrawable : IDrawable
    {
        /// <summary>
        ///     The screen width - will be computed later.
        /// </summary>
        public float Width = 800;

        /// <summary>
        ///     The screen height - will be computed later.
        /// </summary>
        public float Height = 800;

        /// <summary>
        ///     The WorldModel to be drawn.
        /// </summary>
        public World world;
        public GameObject gameObject;

        /// <summary>
        ///     The Client Player fields.
        /// </summary>
        private float worldClientPlayerX;
        private float worldClientPlayerY;
        private float worldClientPlayerRadius;
        public float screenClientPlayerX;
        public float screenClientPlayerY;
        private int clientPlayerColor;

        /// <summary>
        ///     The screen fields.
        /// </summary>
        float leftScreenX;
        float rightScreenX;
        float topScreenY;
        float bottomScreenY;

        /// <summary>
        ///     Zoom numbers that will be used to calculate zooming.
        /// </summary>
        float zoomConstant = 50;
        float zoomRatio;

        /// <summary>
        ///     The logger object that will be used for debugging purposes.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        ///     Constructor of WorldDrawable
        /// </summary>
        public WorldDrawable(ILogger logger)
        {
            world = new();
            gameObject = new();

            _logger = logger;
            world.logger = _logger;
        }

        /// <summary>
        ///     Draws a circle based on the WorldModel object data.
        /// </summary>
        /// <param name="canvas"> The canvas object to draw on. </param>
        /// <param name="dirtyRect"> The area of the canvas that needs to be redrawn. </param>
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            DrawPlaySurface(canvas);

            if (world.FoodList.Count > 0 && world.ClientPlayer != null)
            {
                DrawFoods(canvas, world.FoodList);
            }
            if (world.PlayerList.Count > 0 && world.ClientPlayer != null)
            {
                DrawPlayers(canvas, world.PlayerList);
            }
        }

        /// <summary>
        ///     A helper method to draw a play surface.
        /// </summary>
        /// <param name="canvas"> The canvas object to draw on. </param>
        private void DrawPlaySurface(ICanvas canvas)
        {
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;
            canvas.FillColor = Colors.GhostWhite;
            canvas.FillRectangle(0, 0, 800, 800);

            _logger.LogInformation($"Drawing the play surface on screen.");
        }

        /// <summary>
        ///     A helper method to draw foods.
        /// </summary>
        /// <param name="canvas"> The canvas object to draw on. </param>
        /// <param name="foods"> The food objects that will be drawn on canvas. </param>
        private void DrawFoods(ICanvas canvas, Dictionary<long, Food> foods)
        {
            // Locking food to prevent race condition.
            lock (foods)
            {
                foreach (var food in foods.Values)
                {
                    // Assign values onto the player variables
                    worldClientPlayerX = world.ClientPlayer.X;
                    worldClientPlayerY = world.ClientPlayer.Y;
                    worldClientPlayerRadius = world.ClientPlayer.CircleRadius;
                    clientPlayerColor = world.ClientPlayer.ARGBColor;

                    // Screen variables - 800 x 800 (Width * Height)
                    leftScreenX = worldClientPlayerX - Width / 2;
                    rightScreenX = worldClientPlayerY + Width / 2;
                    topScreenY = worldClientPlayerY - Height / 2;  
                    bottomScreenY = worldClientPlayerY + Height / 2;
                   
                    // Food variables
                    float worldFoodX = food.X;
                    float worldFoodY = food.Y;
                    float worldFoodRadius = food.CircleRadius;
                    float screenFoodRadius = worldFoodRadius * zoomRatio;
                    int foodColor = food.ARGBColor;

                    // Convert the food coordinates from world to screen.
                    ConvertFromWorldToScreenFoodOrPlayer(worldFoodX, worldFoodY, out float screenFoodX, out float screenFoodY);

                    bool IsInTheScreenCoordinates = (screenFoodX <= screenClientPlayerX + rightScreenX && screenFoodX >= screenClientPlayerX - leftScreenX)
                                                && (screenFoodY <= screenClientPlayerY + topScreenY && screenFoodY >= screenClientPlayerY - bottomScreenY);

                    // If the food is in the screen coordinates, you draw it.
                    if (IsInTheScreenCoordinates)
                    {
                        // Draw the food on canvas.
                        canvas.FillColor = Color.FromInt(foodColor);
                        canvas.FillCircle(screenFoodX, screenFoodY, screenFoodRadius);
                    }
                }
            }
            _logger.LogInformation($"Just drew the food lists on screen. They will be keep on updating.");
        }

        /// <summary>
        ///     A helper method to draw players.
        /// </summary>
        /// <param name="canvas"> The canvas object to draw on. </param>
        /// <param name="players"> The player objects that will be drawn on canvas. </param>
        private void DrawPlayers(ICanvas canvas, Dictionary<long, Player> players)
        {
            // Locking players to prevent race condition.
            lock (players)
            {
                foreach (var player in players.Values)
                {
                    // Assign values onto the player variables

                    float worldPlayerRadius = player.CircleRadius;
                    int playerColor = player.ARGBColor;

                    // Keep track of offsets
                    leftScreenX = worldClientPlayerX - Width / 2;
                    rightScreenX = worldClientPlayerY + Width / 2;
                    topScreenY = worldClientPlayerY - Height / 2;
                    bottomScreenY = worldClientPlayerY + Height / 2;

                    if (player.ID == world.PlayerID)
                    {
                        ConvertFromWorldToScreenClientPlayer(worldClientPlayerX, worldClientPlayerY, out screenClientPlayerX, out screenClientPlayerY);

                        // Draw the ClientPlayer on canvas.
                        canvas.FillColor = Color.FromInt(clientPlayerColor);
                        canvas.FillCircle(screenClientPlayerX, screenClientPlayerY, worldPlayerRadius);
                        canvas.DrawString(player.Name, screenClientPlayerX, screenClientPlayerY, HorizontalAlignment.Center);

                        Width = worldClientPlayerRadius * zoomConstant;
                        Height = worldClientPlayerRadius * zoomConstant;
                    }
                    else
                    {
                        float worldPlayerX = player.X;
                        float worldPlayerY = player.Y;
                        float screenPlayerRadius = worldPlayerRadius * zoomRatio;

                        // Convert the player coordinates from world to screen.
                        ConvertFromWorldToScreenFoodOrPlayer(worldPlayerX, worldPlayerY, out float screenPlayerX, out float screenPlayerY);

                        bool IsInTheScreenCoordinates = (screenPlayerX <= screenClientPlayerX + rightScreenX && screenPlayerX >= screenClientPlayerX - leftScreenX)
                            && (screenPlayerY <= screenClientPlayerY + topScreenY && screenPlayerY >= screenClientPlayerY - bottomScreenY);

                        if (IsInTheScreenCoordinates)
                        {
                            // Draw the player on canvas.
                            canvas.FillColor = Color.FromInt(playerColor);
                            canvas.FillCircle(screenPlayerX, screenPlayerY, screenPlayerRadius);
                            canvas.DrawString(player.Name, screenPlayerX, screenPlayerY, HorizontalAlignment.Center);
                        }
                    }
                }
            }

            _logger.LogInformation($"Just drew the player lists on screen. They will be keep on updating.");
        }

        /// <summary>
        ///      Converts a food object's position from world coordinates 
        ///      to screen coordinates.
        /// </summary>
        /// <param name="worldCircleX">The X coordinate of the food object in world coordinates.</param>
        /// <param name="worldCircleY">The Y coordinate of the food object in world coordinates.</param>
        /// <param name="screenCircleX">The X coordinate of the food object in screen coordinates.</param>
        /// <param name="screenCircleY">The Y coordinate of the food object in screen coordinates.</param>
        public void ConvertFromWorldToScreenFoodOrPlayer(
            in float worldCircleX, in float worldCircleY, out float screenCircleX, out float screenCircleY)
        {
            // Update zoom ratio.
            zoomRatio = 800 / Width; 

            // Calculate screen coordinates of food/player objects.
            screenCircleX = (worldCircleX - leftScreenX) * zoomRatio;
            screenCircleY = (worldCircleY - topScreenY) * zoomRatio;
        }

        /// <summary>
        ///     Converts a client object's position from world coordinates
        ///     to screen coordinates.
        /// </summary>
        /// <param name="worldClientPlayerX">The X coordinate of the client player in world.</param>
        /// <param name="worldClientPlayerY">The Y coordinate of the client player in world.</param>
        /// <param name="screenClientPlayerX">The X coordinate of the client player in screen.</param>
        /// <param name="screenClientPlayerY">The Y coordinate of the client player in screen.</param>
        public void ConvertFromWorldToScreenClientPlayer(in float worldClientPlayerX, in float worldClientPlayerY, out float screenClientPlayerX, out float screenClientPlayerY)
        {
            // Calculate the client player coordinates.
            screenClientPlayerX = (Width / 2) * zoomRatio;
            screenClientPlayerY = (Height / 2) * zoomRatio;
        }
    }
}
