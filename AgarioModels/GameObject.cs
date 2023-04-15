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
    ///     A class representing a generic game object in the Agario game.
    /// </summary>
    public class GameObject
    {
        /// <summary>
        ///     Gets the unique identifier of the game object.
        /// </summary>
        public long ID { get; private set; }

        /// <summary>
        ///     Gets the location of the game object as a 2D vector.
        /// </summary>
        public Vector2 Location { get; private set; }

        /// <summary>
        ///     Gets the X coordinate of the game object in the game world.
        /// </summary>
        public float X { get { return Location.X; } }

        /// <summary>
        ///     Gets the Y coordinate of the game object in the game world.
        /// </summary>
        public float Y { get { return Location.Y; } }

        /// <summary>
        ///     Gets the ARGB color of the game object.
        /// </summary>
        public int ARGBColor { get; private set; }

        /// <summary>
        ///     Gets or sets the radius of the game object's circular shape.
        /// </summary>
        public float CircleRadius { get; private set; } = 3;

        /// <summary>
        ///     Gets the mass of the game object, which is calculated from its radius.
        /// </summary>
        public float Mass
        {
            get
            {
                if (_mass > 0)
                    return _mass;
                else
                    return (float)(Math.PI * CircleRadius * CircleRadius);
            }
            private set { _mass = value; }
        }

        /// <summary>
        ///     Backing field for the Mass property.
        /// </summary>
        private float _mass;

        /// <summary>
        ///     A class representing a generic game object in the Agario game.
        /// </summary>
        /// <param name="ID">The unique identifier of the game object.</param>
        /// <param name="x">The X coordinate of the game object in the game world</param>
        /// <param name="y">The Y coordinate of the game object in the game world.</param>
        /// <param name="ARGBcolor">The ARGB color of the game object.</param>
        /// <param name="Mass">The mass of the game object.</param>
        public GameObject(long ID, float x, float y, int ARGBcolor, float Mass)
        {
            this.ID = ID;
            this.Location = new Vector2(x, y);
            this.ARGBColor = ARGBcolor;
            this.Mass = Mass;
        }

        /// <summary>
        ///     Constructs a new, empty instance of the GameObject class.
        /// </summary>
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