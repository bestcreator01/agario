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
        public long ID { get; private set; }
        public Vector2 Location { get; private set; }
        public float X { get { return Location.X; } }
        public float Y { get { return Location.Y; } }
        public int ARGBColor { get; private set; }
        public float CircleRadius { get; private set; }
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
        ///     Fields
        /// </summary>
        private float _mass;

        /// <summary>
        ///     Constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="ARGBcolor"></param>
        /// <param name="circleRadius"></param>
        public GameObject(long ID, float x, float y, int ARGBcolor, float Mass)
        {
            this.ID = ID;
            this.Location = new Vector2(x, y);
            this.ARGBColor = ARGBcolor;
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
