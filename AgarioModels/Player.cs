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
///     This class contains the details of the Player object.
///     
/// </summary>
namespace AgarioModels
{
    /// <summary>
    ///     A class representing a palyer in Agar.io.
    /// </summary>
    public class Player : GameObject
    {
        /// <summary>
        ///     The name of the player.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates a new instance of the Player class with the specified parameters.
        /// </summary>
        /// <param name="ID">The ID of the player.</param>
        /// <param name="X">The X-coordinate of the player's location.</param>
        /// <param name="Y">The Y-coordinate of the player's location.</param>
        /// <param name="ARGBColor">The color of the player's avatar, specified as an ARGB value.</param>
        /// <param name="Mass">The mass of the player's avatar.</param>
        [JsonConstructor]
        public Player(long ID, float X, float Y, int ARGBColor, float Mass) : base(ID, X, Y, ARGBColor, Mass)
        {
        }

    }
}
