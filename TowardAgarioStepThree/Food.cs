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
///     This class contains details of the Food object.
///     
/// </summary>
namespace TowardAgarioStepThree
{
    /// <summary>
    ///     A class representing a food object in the Agario world.
    /// </summary>
    public class Food
    {
        /// <summary>
        ///     The x-coordinate of the food.
        /// </summary>
        public float X { get; set; }

        /// <summary>
        ///     The y-coordinate of the food.
        /// </summary>
        public float Y { get; set; }

        /// <summary>
        ///     The ARGB color code of the food.
        /// </summary>
        public int ARGBcolor { get; set; }

        /// <summary>
        ///     The mass of the food.
        /// </summary>
        public float Mass { get; set; }

    }
}
