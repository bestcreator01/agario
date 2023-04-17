using AgarioModels;
using Microsoft.Extensions.Logging;
using System.Collections;

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
        public readonly float Width = 800;

        /// <summary>
        ///     The screen height - will be computed later.
        /// </summary>
        public readonly float Height = 800;

        /// <summary>
        ///     The WorldModel to be drawn.
        /// </summary>
        public World world;
        public GameObject gameObject;

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
            if (world.ClientPlayer != null)
            {

                // Center the clientPlayer on the PlaySurface.
                float clientPlayerScreenX = Width / 2;
                float clientPlayerScreenY = Height / 2;

                if (world.ClientPlayer.X + world.ClientPlayer.CircleRadius <= world.WorldWidth
                    && world.ClientPlayer.X - world.ClientPlayer.CircleRadius >= 0)
                {
                    // TODO - Draw the player on the center.
                }

                // TODO - Convert the coordinates of the rest of the players.
                // TODO - Draw the rest of the players.

                // TODO - Scale the size of foods and players.

            }


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
            canvas.FillRectangle(0, 0, Width, Height);

            _logger.LogInformation($"Drawing the play surface on screen.");
        }

        /// <summary>
        ///     A helper method to draw foods.
        /// </summary>
        /// <param name="canvas"> The canvas object to draw on. </param>
        /// <param name="foods"> The food objects that will be drawn on canvas. </param>
        private void DrawFoods(ICanvas canvas, Dictionary<long, Food> foods)
        {
            lock (world.FoodList)
            {
                // Convert the coordinates of foods.
                // Draw all the foods in the screen.
                foreach (var food in foods.Values)
                {
                    // Food variables
                    float worldFoodX = food.X;
                    float worldFoodY = food.Y;
                    float worldFoodRadius = food.CircleRadius;
                    int foodColor = food.ARGBColor;

                    // Player variables
                    float worldPlayerX = world.ClientPlayer.X;
                    float worldPlayerY = world.ClientPlayer.Y;
                    float worldPlayerRadius = world.ClientPlayer.CircleRadius;
                    int playerColor = world.ClientPlayer.ARGBColor;

                    // Convert the food coordinates from world to screen.
                    ConvertFromWorldToScreenFoodOrPlayer(worldFoodX, worldFoodY, world.WorldWidth, world.WorldHeight, 
                                                        out int screenFoodX, out int screenFoodY, Width, Height);

                    // Convert the player coordinates from world to screen.
                    ConvertFromWorldToScreenFoodOrPlayer(worldPlayerX, worldPlayerY, world.WorldWidth, world.WorldHeight,
                                                        out int screenPlayerX, out int screenPlayerY, Width, Height);
                    
                    float leftXOfScreen = world.ClientPlayer.X - Width / 2;
                    float rightXOfScreen = world.ClientPlayer.X + Width / 2;

                    // If the food is in the screen coordinates, you draw it.
                    if (food.X >= leftXOfScreen || food.X <= rightXOfScreen)
                    {
                        // TODO - Draw the food.
                        // TODO - Convert the coordinates from world to screen.
                    }
                }


                foreach (var food in foods.Values)
                {
                    // Assign the parameters you need for drawing objects.
                    float worldCircleX = food.X;
                    float worldCircleY = food.Y;
                    float radius = food.CircleRadius;
                    int color = food.ARGBColor;
                    ConvertFromWorldToScreenFoodOrPlayer(worldCircleX, worldCircleY, world.WorldWidth, world.WorldHeight, out int screenX, out int screenY, Width, Height);

                    // Draw on canvas
                    canvas.FillColor = Color.FromInt(color);
                    canvas.FillCircle(screenX, screenY, radius);
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
            lock (world.PlayerList)
            {
                foreach (var player in players.Values)
                {
                    // Assign the parameters you need for drawing objects.
                    float worldCircleX = 0; float worldCircleY = 0; float radius = 0; int color = 0;

                    worldCircleX = player.X;
                    worldCircleY = player.Y;
                    color = player.ARGBColor;
                    ConvertFromWorldToScreenFoodOrPlayer(worldCircleX, worldCircleY, world.WorldWidth, world.WorldHeight, out int screenCircleX, out int screenCircleY, Width, Height);
                    radius = (float)(Math.Sqrt(player.Mass / Math.PI));

                    // Draw on canvas
                    canvas.FillColor = Color.FromInt(color);
                    canvas.FillCircle(screenCircleX, screenCircleY, radius);
                    canvas.DrawString(player.Name, screenCircleX, screenCircleY, HorizontalAlignment.Center);
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
        out int screen_w, out int screen_h)
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
            out int screenCircleX, out int screenCircleY, in float screenWidth, in float screenHeight)
        {
            float w_percentage = (float)(worldCircleX / worldWidth);
            float h_percentage = (float)(worldCircleY / worldHeight);

            screenCircleX = (int)(w_percentage * screenWidth);
            screenCircleY = (int)(h_percentage * screenHeight);
        }
    }
}
