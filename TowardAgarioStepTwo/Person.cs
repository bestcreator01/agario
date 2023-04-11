using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
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
///     This class contains the person object.
///     
/// </summary>
namespace TowardAgarioStepTwo
{
    /// <summary>
    ///     
    /// </summary>
    [JsonDerivedType(typeof(Person), typeDiscriminator: "Person")]
    [JsonDerivedType(typeof(Student), typeDiscriminator: "Student")]
    public class Person
    {
        /// <summary>
        ///     
        /// </summary>
        private static int nextId = 0;

        /// <summary>
        ///     
        /// </summary>
        public float GPA { get; protected set; } = 4;

        /// <summary>
        ///     
        /// </summary>
        public int ID { get; protected set; }

        /// <summary>
        ///     
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
