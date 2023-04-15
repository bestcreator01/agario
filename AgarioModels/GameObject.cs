using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

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
///     This class contains details of the game object inside Agario. It allows Food and Player
///     to implement it.
///     
/// </summary>
namespace AgarioModels
{
    /// <summary>
    ///     This class represents a game object.
    /// </summary>
    public class GameObject
    {
        /// <summary>
        ///     Gets or sets the unique identifier for the object.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        ///     Gets or sets the location of the object in 2D space.
        /// </summary>
        public Vector2 Location { get; private set; }

        /// <summary>
        ///     Gets the X-coordinate of the object's location.
        /// </summary>
        public float X
        {
            get
            {
                return Location.X;
            }
        }

        /// <summary>
        ///     Gets the Y-coordinate of the object's location.
        /// </summary>
        public float Y 
        { 
            get 
            { 
                return Location.Y; 
            } 
        }

        /// <summary>
        ///     Gets or sets the ARGB color value of the object.
        /// </summary>
        public int ARGBcolor { get; private set; }

        /// <summary>
        ///     Gets or sets the radius of the circle.
        /// </summary>
        public float CircleRadius { get; private set; }

        /// <summary>
        ///     Gets or sets the mass of the object.
        /// </summary>
        public float Mass 
        { 
            get 
            {
                return (float)(Math.PI * CircleRadius * CircleRadius);
            }

            private set { }
        }

        [JsonConstructor]
        public GameObject(long ID, float X, float Y, int ARGBcolor, float Mass)
        {
            this.ID = ID;
            this.Location = new Vector2(X, Y);
            this.ARGBcolor = ARGBcolor;
            this.Mass = Mass;
        }

        public GameObject()
        {

        }

        /// <summary>
        ///     In this method add the direction vector to the circle.
        ///     If the circle moves off the rectangle to the left or right,
        ///     change the direction vector by multiplying the X direction by a negative one.
        ///     If the circle moves off the rectangle to the top or bottom, multiply the direction.Y by -1.
        /// </summary>
        public void AdvanceGameOneStep()
        {
            float windowWidth = 5000;
            float windowHeight = 5000;

            // Add the direction vector to the circle. ????
            Location += Location;

            // If the circle moves off the rectangle to the left or right,
            // change the direction vector by multiplying the X direction by a negative one.
            if (X - CircleRadius < 0 || X + CircleRadius > windowWidth)
            {
                Location = new Vector2(-Location.X, Location.Y);
            }

            // If the circle moves off the rectangle to the top or bottom,
            // multiply the direction Y by -1.
            if (Y - CircleRadius < 0 || Y + CircleRadius > windowHeight)
            {
                Location = new Vector2(Location.X, -Location.Y);
            }
        }
    }
}
