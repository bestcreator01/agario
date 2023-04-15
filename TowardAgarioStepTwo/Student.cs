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
///     This class contains the Student object.
///     
/// </summary>
namespace TowardAgarioStepTwo
{
    /// <summary>
    ///     A derived class from Person representing a student.
    /// </summary>
    public class Student : Person
    {
        /// <summary>
        ///     Constructor for the Student class.
        /// </summary>
        /// <param name="Name">The name of the student.</param>
        /// <param name="GPA">The GPA of the student.</param>
        /// <param name="ID">The ID of the student.</param>
        public Student(string Name, float GPA, int ID) : base(Name)
        {
        }
    }
}
