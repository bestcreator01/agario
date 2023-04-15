using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.Json;
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
///     This class contains the details of a Food object.
///     
/// </summary>
namespace AgarioModels
{
    /// <summary>
    ///     A class representing a food object in the game world.
    /// </summary>
    public class Food : GameObject
    {
        /// <summary>
        ///     Constructs a new instance of the Food class with the specified parameters.
        /// </summary>
        /// <param name="ID">The unique identifier of the food object.</param>
        /// <param name="X">The X coordinate of the food object in the game world.</param>
        /// <param name="Y">The Y coordinate of the food object in the game world.</param>
        /// <param name="ARGBColor">The color of the food object in ARGB format.</param>
        /// <param name="Mass">The mass of the food object.</param>
        [JsonConstructor]
        public Food(long ID, float X, float Y, int ARGBColor, float Mass) : base(ID, X, Y, ARGBColor, Mass)
        {
        }
    }
}
