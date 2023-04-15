using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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
///     This class contains the details of the Player object.
///     
/// </summary>
namespace AgarioModels
{
    public class Player : GameObject
    {
        /// <summary>
        ///     Player elements (fields).
        /// </summary>
        private long   PlayerID;
        private float  PlayerX;
        private float  PlayerY;
        private int    PlayerARGBcolor;
        private float  PlayerMass;
        public string Name = "";

        [JsonConstructor]
        public Player(long ID, float X, float Y, int ARGBcolor, float Mass) : base(ID, X, Y, ARGBcolor, Mass)
        {
            this.PlayerID = ID;
            this.PlayerX = X;
            this.PlayerY = Y;
            this.PlayerARGBcolor = ARGBcolor;
            this.PlayerMass = Mass;
        }

        public Player()
        {

        }
    }
}
