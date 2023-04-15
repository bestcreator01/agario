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
///     This should implement the IDrawable interface, and have a WorldModel field.
///     
/// </summary>
namespace TowardAgarioStepOne
{
    /// <summary>
    ///     This class should implement the IDrawable interface, and have a World Model field.
    /// </summary>
    public class WorldDrawable : IDrawable
    {
        /// <summary>
        ///     The WorldModel to be drawn.
        /// </summary>
        public WorldModel worldModel;

        /// <summary>
        ///     Constructor of WorldDrawable
        /// </summary>
        public WorldDrawable()
        {
            worldModel = new WorldModel();
        }

        /// <summary>
        ///     Draws a circle based on the WorldModel object data.
        /// </summary>
        /// <param name="canvas"> The canvas object to draw on. </param>
        /// <param name="dirtyRect"> The area of the canvas that needs to be redrawn. </param>
        public void Draw(ICanvas canvas, RectF dirtyRect)
        {
            float x = worldModel.CircleX;
            float y = worldModel.CircleY;
            float radius = worldModel.CircleRadius;

            canvas.StrokeColor = Colors.Black;
            canvas.StrokeSize = 2;
            canvas.FillColor = Colors.LightSkyBlue;
            canvas.FillCircle(x, y, radius);
        }
    }
}
