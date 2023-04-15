using System.Numerics;

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
///     This contains the "global state" of our program, which has the x, y, a radius of a circle,
///     the direction that the circle is moving in, and the width and height of the "window".
///         
/// </summary>

namespace TowardAgarioStepOne
{
    /// <summary>
    ///     This class represents the "global state" of our program, 
    ///     containing information about the circle's position, size, 
    ///     and direction, as well as the dimensions of the window.
    /// </summary>
    public class WorldModel
    {
        /// <summary>
        ///     The x-coordinate of the center of the circle.
        /// </summary>
        public float CircleX { get; set; } = 100;

        /// <summary>
        ///     The y-coordinate of the center of the circle. 
        /// </summary>
        public float CircleY { get; set; } = 100;

        /// <summary>
        ///     The radius of the circle.
        /// </summary>
        public float CircleRadius { get; set; } = 50;

        /// <summary>
        ///     The direction that the circle is currently moving in,
        ///     represented as a Vector2.
        /// </summary>
        public Vector2 CircleDirection { get; set; } = new Vector2(50, 25);

        /// <summary>
        ///     The width of the window in pixels.
        /// </summary>
        public readonly int WindowWidth = 5000;

        /// <summary>
        ///     The height of the window in pixels.
        /// </summary>
        public readonly int WindowHeight = 5000;

        /// <summary>
        ///     In this method add the direction vector to the circle.
        ///     If the circle moves off the rectangle to the left or right,
        ///     change the direction vector by multiplying the X direction by a negative one.
        ///     If the circle moves off the rectangle to the top or bottom, multiply the direction.Y by -1.
        /// </summary>
        public void AdvanceGameOneStep()
        {
            // Add the direction vector to the circle.
            CircleX += CircleDirection.X;
            CircleY += CircleDirection.Y;

            // If the circle moves off the rectangle to the left or right,
            // change the direction vector by multiplying the X direction by a negative one.
            if (CircleX - CircleRadius < 0 || CircleX + CircleRadius > WindowWidth)
            {
                CircleDirection = new Vector2(-CircleDirection.X, CircleDirection.Y);
            }

            // If the circle moves off the rectangle to the top or bottom,
            // multiply the direction Y by -1.
            if (CircleY - CircleRadius < 0 || CircleY + CircleRadius > WindowHeight)
            {
                CircleDirection = new Vector2(CircleDirection.X, -CircleDirection.Y);
            }
        }
    }
}