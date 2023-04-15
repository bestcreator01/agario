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

        ///// <summary>
        /////     
        ///// </summary>
        //private IEnumerable<Food> foodList;
        //private IEnumerable<Player> playerList;

        /// <summary>
        ///     Constructor of WorldDrawable
        /// </summary>
        public WorldDrawable(GraphicsView gv)
        {
            world = new();
            gameObject = new();
            this.gv = gv;
            //foodList = world.FoodList.Values;
            //playerList = world.PlayerList.Values;
        }

        /// <summary>
        ///     Draws a circle based on the WorldModel object data.
        /// </summary>
        /// <param name="canvas"> The canvas object to draw on. </param>
        /// <param name="dirtyRect"> The area of the canvas that needs to be redrawn. </param>
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            DrawPlaySurface(canvas);

            //DrawGameObject(canvas, foodList);
            DrawGameObject(canvas, world.FoodList);

            //DrawGameObject(canvas, playerList);
            DrawGameObject(canvas, world.PlayerList);
        }

        /// <summary>
        ///     A helper method to draw a play surface.
        /// </summary>
        /// <param name="canvas"> The canvas object to draw on. </param>
        private void DrawPlaySurface(ICanvas canvas)
        {
            ConvertFromWorldToScreen(world.WindowWidth, world.WindowHeight, out int Width, out int Height);
            canvas.FillColor = Colors.LightPink;
            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;
            canvas.FillRectangle(0, 0, Width, Height);
        }

        /// <summary>
        ///     A helper method to draw game objects.
        /// </summary>
        /// <param name="canvas"> The canvas object to draw on. </param>
        private void DrawGameObject(ICanvas canvas, IEnumerable gameObject)
        {
            foreach (var obj in gameObject)
            {
                // Assign the parameters you need for drawing objects.
                float x = 0; float y = 0; float radius = 0; int color = 0;

                // Declare the values of x, y, and radius depending on what object it is.
                if (obj is Food food)
                {
                    x = food.X;
                    y = food.Y;
                    radius = food.CircleRadius;
                    color = food.ARGBColor;
                }
                else if (obj is Player player)
                {
                    x = player.X;
                    y = player.Y;
                    radius = player.CircleRadius; // TODO - Set the circle radius of players bigger than foods.
                    color = player.ARGBColor;
                }

                // Draw on canvas.
                canvas.FillColor = Color.FromInt(color);
                canvas.FillCircle(x, y, radius);
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
    }
}
