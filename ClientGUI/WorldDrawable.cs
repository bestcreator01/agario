using AgarioModels;

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
        private readonly float Width = 500;

        /// <summary>
        ///     The screen height - will be computed later.
        /// </summary>
        private readonly float Height = 500;

        /// <summary>
        ///     World that we have a reference to.
        /// </summary>
        private readonly CheckBox moveOnInvalidateCheckBox;
        private readonly CheckBox invalidateAlwaysCheckBox;
        private readonly GraphicsView gv;

        /// <summary>
        ///     The WorldModel to be drawn.
        /// </summary>
        public World world;
        public GameObject gameObject;

        /// <summary>
        ///     Constructor of WorldDrawable
        /// </summary>
        public WorldDrawable(CheckBox moveOnIvalidateCheckBox, CheckBox invalidateAlwaysCheckBox, GraphicsView gv)
        {
            world = new World();
            gameObject = new GameObject();
            this.moveOnInvalidateCheckBox = moveOnIvalidateCheckBox;
            this.invalidateAlwaysCheckBox = invalidateAlwaysCheckBox;
            this.gv = gv;
        }

        /// <summary>
        /// 
        /// </summary>
        public WorldDrawable()
        {
            world = new World();
            gameObject = new GameObject();
        }

        /// <summary>
        ///     Draws a circle based on the WorldModel object data.
        /// </summary>
        /// <param name="canvas"> The canvas object to draw on. </param>
        /// <param name="dirtyRect"> The area of the canvas that needs to be redrawn. </param>
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {

            foreach (var food in world.FoodList)
            {
                //ConvertFromWorldToScreen(food.X, food.Y);
                float x = food.X;
                float y = food.Y;
                float radius = food.CircleRadius;

                canvas.StrokeColor = Colors.Black;
                canvas.StrokeSize = 2;
                canvas.FillColor = Colors.LightSkyBlue;
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
        /// <param name="world_x"></param>
        /// <param name="world_y"></param>
        /// <param name="world_w"></param>
        /// <param name="world_h"></param>
        /// <param name="screen_x"></param>
        /// <param name="screen_y"></param>
        /// <param name="screen_w"></param>
        /// <param name="screen_h"></param>
        private void ConvertFromWorldToScreen(
        in float world_x, in float world_y, in float world_w, in float world_h,
        out int screen_x, out int screen_y, out int screen_w, out int screen_h)
        {
            // TODO - calculate the screen coordinates based on the lecture slides
            screen_x = (int)(world_x / 3000.0F * this.Width);
            screen_y = (int)(world_y / 2000.0F * this.Height);
            screen_w = (int)(world_w / 3000.0F * this.Width);
            screen_h = (int)(world_h / 2000.0F * this.Height);
        }
    }
}
