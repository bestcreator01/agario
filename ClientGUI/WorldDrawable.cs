using AgarioModels;
using System.Collections;

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
        private readonly float Width = 800;

        /// <summary>
        ///     The screen height - will be computed later.
        /// </summary>
        private readonly float Height = 800;

        /// <summary>
        ///     World that we have a reference to.
        /// </summary>
        private readonly GraphicsView gv;

        /// <summary>
        ///     The WorldModel to be drawn.
        /// </summary>
        public World world;
        public GameObject gameObject;

        /// <summary>
        ///     The screen width.
        /// </summary>
        private int screenWidth;

        /// <summary>
        ///     The screen height.
        /// </summary>
        private int screenHeight;

        /// <summary>
        ///     Constructor of WorldDrawable
        /// </summary>
        public WorldDrawable(GraphicsView gv)
        {
            world = new();
            gameObject = new();
            this.gv = gv;
        }

        /// <summary>
        ///     Draws a circle based on the WorldModel object data.
        /// </summary>
        /// <param name="canvas"> The canvas object to draw on. </param>
        /// <param name="dirtyRect"> The area of the canvas that needs to be redrawn. </param>
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            DrawPlaySurface(canvas);

            if (world.FoodList.Count > 0)
            {
                DrawFoods(canvas, world.FoodList);
            }
            if (world.PlayerList.Count > 0)
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
            ConvertFromWorldToScreen(world.WindowWidth, world.WindowHeight, out screenWidth, out screenHeight);
            canvas.FillColor = Colors.GhostWhite;
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;
            canvas.FillRectangle(0, 0, Width, Height);
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="foods"></param>
        private void DrawFoods(ICanvas canvas, Dictionary<long, Food> foods)
        {
            var copyList = foods.Values;
            lock (copyList)
            {
                foreach (var food in copyList)
                {
                    // Assign the parameters you need for drawing objects.
                    float worldCircleX = 0; float worldCircleY = 0; float radius = 0; int color = 0;

                    worldCircleX = food.X;
                    worldCircleY = food.Y;
                    radius = food.CircleRadius;
                    color = food.ARGBColor;
                    ConvertFromWorldToScreenFoodAndPlayer(worldCircleX, worldCircleY, world.WindowWidth, world.WindowHeight, out int screenX, out int screenY, screenWidth, screenHeight);

                    // Draw on canvas
                    canvas.FillColor = Color.FromInt(color);
                    canvas.FillCircle(screenX, screenY, radius);
                }
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="canvas"></param>
        /// <param name="players"></param>
        private void DrawPlayers(ICanvas canvas, Dictionary<long,  Player> players)
        {
            var copyList = players.Values;
            lock (copyList)
            {
                foreach (var player in copyList)
                {
                    // Assign the parameters you need for drawing objects.
                    float worldCircleX = 0; float worldCircleY = 0; float radius = 0; int color = 0;

                    worldCircleX = player.X;
                    worldCircleY = player.Y;
                    color = player.ARGBColor;
                    ConvertFromWorldToScreenFoodAndPlayer(worldCircleX, worldCircleY, world.WindowWidth, world.WindowHeight, out int screenCircleX, out int screenCircleY, screenWidth, screenHeight);
                    radius = (float)(Math.Sqrt(player.Mass / Math.PI)); // TODO - Set the circle radius of players bigger than foods.

                    // Draw on canvas
                    canvas.FillColor = Color.FromInt(color);
                    canvas.FillCircle(screenCircleX, screenCircleY, radius);
                    canvas.DrawString(player.Name, screenCircleX, screenCircleY, HorizontalAlignment.Center);
                }
            }
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
        private void ConvertFromWorldToScreenFoodAndPlayer(
            in float worldCircleX, in float worldCircleY, in float worldWidth, in float worldHeight,
            out int screenCircleX, out int screenCircleY, in int screenWidth, in int screenHeight)
        {
            float w_percentage = (float)(worldCircleX / worldWidth);
            float h_percentage = (float)(worldCircleY / worldHeight);

            screenCircleX = (int)(w_percentage * screenWidth);
            screenCircleY = (int)(h_percentage * screenHeight);
        }
    }
}
