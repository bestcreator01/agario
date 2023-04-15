using System.Text.Json.Serialization;

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
///     This class contains the Person object.
///     
/// </summary>
namespace TowardAgarioStepTwo
{
    /// <summary>
    ///     A base class representing a person.
    /// </summary>
    [JsonDerivedType(typeof(Person), typeDiscriminator: "Person")]
    [JsonDerivedType(typeof(Student), typeDiscriminator: "Student")]
    public class Person
    {
        /// <summary>
        ///     A static field to keep track of the next available ID for a person.  
        /// </summary>
        private static int nextId = 0;

        /// <summary>
        ///     The GPA (grade point average) of the person.  
        /// </summary>
        public float GPA { get; protected set; } = 4;

        /// <summary>
        ///     The unique identifier of the person.
        /// </summary>
        public int ID { get; protected set; }

        /// <summary>
        ///     The name of the person. 
        /// </summary>
        public string Name { get; protected set; } = "Jim";

        /// <summary>
        ///     Default constructor of Person.
        /// </summary>
        public Person(string Name)
        {
            this.Name = Name;
            this.ID = nextId++;
        }
    }
}