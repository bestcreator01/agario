using Microsoft.Extensions.Logging;

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
///     This class contains the global state of the Agario world.
///     
/// </summary>
namespace AgarioModels
{
    /// <summary>
    /// 
    /// </summary>
    public class World
    {
        /// <summary>
        ///     The width of the window in pixels.
        /// </summary>
        public readonly float WindowWidth = 5000;

        /// <summary>
        ///     The height of the window in pixels.
        /// </summary>
        public readonly float WindowHeight = 5000;

        /// <summary>
        ///     A list of players. Contains ID and player.
        /// </summary>
        public Dictionary<long, Player> PlayerList = new();

        /// <summary>
        ///     A list of Food objects. Contains ID and food.
        /// </summary>
        public Dictionary<long, Food> FoodList = new();

        /// <summary>
        ///     A logger object.
        /// </summary>
        public readonly ILogger logger;
    }
}