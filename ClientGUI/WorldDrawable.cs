using AgarioModels;
using Microsoft.Extensions.Logging;
using System.Collections;
using System.Runtime.CompilerServices;

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
        public float Width = 0;

        /// <summary>
        ///     The screen height - will be computed later.
        /// </summary>
        public float Height = 0;

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
        private float screenClientPlayerX;
        private float screenClientPlayerY;
        private int clientPlayerColor;

        /// <summary>
        ///     The screen fields.
        /// </summary>
        float leftXOfScreen;
        float rightXOfScreen;
        float topYOfScreen;
        float bottomYOfScreen;

        float zoomConstant = 20;

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
            lock (foods)
            {
                foreach (var food in foods.Values)
                {
                    // Assign values onto the player variables
                    worldClientPlayerX = world.ClientPlayer.X;
                    worldClientPlayerY = world.ClientPlayer.Y;
                    worldClientPlayerRadius = world.ClientPlayer.CircleRadius;
                    clientPlayerColor = world.ClientPlayer.ARGBColor;

                    // Food variables
                    float worldFoodX = food.X;
                    float worldFoodY = food.Y;
                    float worldFoodRadius = food.CircleRadius;
                    int foodColor = food.ARGBColor;


                    // Screen variables - 800 x 800 (Width * Height)
                    leftXOfScreen = worldClientPlayerX - Width / 2;
                    rightXOfScreen = worldClientPlayerY + Width / 2;
                    topYOfScreen = worldClientPlayerY - Height / 2;  
                    bottomYOfScreen = worldClientPlayerY + Height / 2;

                    // Convert the food coordinates from world to screen.
                    ConvertFromWorldToScreenFoodOrPlayer(worldFoodX, worldFoodY, world.WorldWidth, world.WorldHeight,
                                                        out float screenFoodX, out float screenFoodY, Width, Height);

                    float w_left_x = world.ClientPlayer.X - (this.Width / 2);
                    float w_left_y = world.ClientPlayer.Y - (this.Height / 2);


                    bool IsInTheScreenCoordinates = (screenFoodX <= screenClientPlayerX + w_left_x && screenFoodX >= screenClientPlayerX - w_left_x)
                                                && (screenFoodY <= screenClientPlayerY + w_left_y && screenFoodY >= screenClientPlayerY - w_left_y);

                    //bool IsInTheScreenCoordinates = (worldFoodX >= leftXOfScreen && worldFoodX <= rightXOfScreen)
                    //                            && (worldFoodY >= topYOfScreen && worldFoodY <= bottomYOfScreen);

                    // If the food is in the screen coordinates, you draw it.
                    if (IsInTheScreenCoordinates)
                    {
                        // Draw the food on canvas.
                        canvas.FillColor = Color.FromInt(foodColor);
                        canvas.FillCircle(screenFoodX, screenFoodY, worldFoodRadius); // ??? - not sure about radius.
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
            lock (players)
            {
                foreach (var player in players.Values)
                {
                    float screenPlayerRadius = player.CircleRadius;
                    int playerColor = player.ARGBColor;

                    if (player.ID == world.PlayerID)
                    {
                        ConvertFromWorldToScreenClientPlayer(world.ClientPlayer.X, world.ClientPlayer.Y, out screenClientPlayerX, out screenClientPlayerY);

                        // Draw the ClientPlayer on canvas.
                        canvas.FillColor = Color.FromInt(clientPlayerColor);
                        canvas.FillCircle(screenClientPlayerX, screenClientPlayerY, worldClientPlayerRadius);
                        canvas.DrawString(player.Name, screenClientPlayerX, screenClientPlayerY, HorizontalAlignment.Center);

                        Width = world.ClientPlayer.CircleRadius * 2 * zoomConstant;
                        Height = world.ClientPlayer.CircleRadius * 2 * zoomConstant;

                        // TODO - Zoom in or out whenever the player's mass or radius changes.
                        //Width += screenPlayerRadius * zoomConstant;
                        //Height += screenPlayerRadius * zoomConstant;
                    }
                    else
                    {
                        float worldPlayerX = player.X;
                        float worldPlayerY = player.Y;

                        // Convert the player coordinates from world to screen.
                        ConvertFromWorldToScreenFoodOrPlayer(worldPlayerX, worldPlayerY, world.WorldWidth, world.WorldHeight,
                                                            out float screenPlayerX, out float screenPlayerY, Width, Height);

                        //bool IsInTheScreenCoordinates = (worldPlayerX >= leftXOfScreen && worldPlayerX <= rightXOfScreen)
                        //                                && (worldPlayerY >= topYOfScreen && worldPlayerY <= bottomYOfScreen);
                        float w_left_x = world.ClientPlayer.X - (this.Width / 2);
                        float w_left_y = world.ClientPlayer.Y - (this.Height / 2);

                        bool IsInTheScreenCoordinates = (screenPlayerX <= screenClientPlayerX + w_left_x && screenPlayerX >= screenClientPlayerX - w_left_x)
                            && (screenPlayerY <= screenClientPlayerY + w_left_y && screenPlayerY >= screenClientPlayerY - w_left_y);

                        if (IsInTheScreenCoordinates)
                        {
                            // Draw the player on canvas.
                            canvas.FillColor = Color.FromInt(playerColor);
                            canvas.FillCircle(screenPlayerX, screenPlayerY, screenPlayerRadius);
                            canvas.DrawString(player.Name, screenPlayerX, screenPlayerX, HorizontalAlignment.Center);
                        }
                    }
                }
            }

            _logger.LogInformation($"Just drew the player lists on screen. They will be keep on updating.");
        }

        /// <summary>
        /// This code converts between world and screen coordinates.
        ///
        /// Assumption: The world is 3000 wide and 2000 high. WARNING: never use magic numbers like these in
        /// your code. Always replaced by named constants that "live" somewhere appropriate.
        ///
        /// Assumption: we are drawing across the entire GUI window. WARNING: you probably will not do this
        /// in your program... leave room for some info displays.
        ///
        /// Assumption: We are drawing the entire world on the GUI window. WARNING: you will need to "shrink"
        /// the area of the "world" that is shown. Think about how to do this and ask questions
        /// in lecture.
        /// </summary>
        /// <param name="world_w"></param>
        /// <param name="world_h"></param>
        /// <param name="screen_w"></param>
        /// <param name="screen_h"></param>
        private void ConvertFromWorldToScreen(
        in float world_w, in float world_h,
        out float screen_w, out float screen_h)
        {
            // Calculate the screen coordinates based on the lecture slides.
            screen_w = (int)(world_w / 5000.0F * Width);
            screen_h = (int)(world_h / 5000.0F * Height);
        }

        /// <summary>
        ///      Converts a food object's position from world coordinates 
        ///      to screen coordinates.
        /// </summary>
        /// <param name="worldCircleX">The X coordinate of the food object in world coordinates.</param>
        /// <param name="worldCircleY">The Y coordinate of the food object in world coordinates.</param>
        /// <param name="worldWidth">The width of the world in world coordinates.</param>
        /// <param name="worldHeight">The height of the world in world coordinates.</param>
        /// <param name="screenCircleX">The X coordinate of the food object in screen coordinates.</param>
        /// <param name="screenCircleY">The Y coordinate of the food object in screen coordinates.</param>
        /// <param name="screenWidth">The width of the screen in pixels.</param>
        /// <param name="screenHeight">The height of the screen in pixels.</param>        
        public void ConvertFromWorldToScreenFoodOrPlayer(
            in float worldCircleX, in float worldCircleY, in float worldWidth, in float worldHeight,
            out float screenCircleX, out float screenCircleY, in float screenWidth, in float screenHeight)
        {
            float w_left_x = world.ClientPlayer.X - (this.Width / 2);
            float w_left_y = world.ClientPlayer.Y - (this.Height / 2);

            screenCircleX = worldCircleX - w_left_x;
            screenCircleY = worldCircleY - w_left_y;
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
            float w_left_x = worldClientPlayerX - (this.Width / 2);
            float w_left_y = worldClientPlayerY - (this.Height / 2);

            float offset_x = worldClientPlayerX - w_left_x;
            float offset_y = worldClientPlayerY - w_left_y;
            float w_percentage_x = offset_x / Width;
            float w_percentage_y = offset_y / Height;

            screenClientPlayerX = w_percentage_x * Width;
            screenClientPlayerY = w_percentage_y * Height;
        }
    }
}
