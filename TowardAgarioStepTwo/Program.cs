// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using TowardAgarioStepTwo;

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
///     This class contains the console program.
///     
/// </summary>


// Serialize a Person.
//var person = new Person("Seoin", 4);
//string message = JsonSerializer.Serialize(person);
//Console.WriteLine(message);

// Serialize a list of Person.
//List<Person> people = new List<Person>();
//people.Add(new Person("Jim", (float)3.0));
//people.Add(new Person("Dav", (float)3.2));
//people.Add(new Person("Erin", (float)3.4));
//people.Add(new Person("Mary", (float)3.6));
//people.Add(new Person("Pat", (float)3.8));

//string message2 = JsonSerializer.Serialize(people, new JsonSerializerOptions { WriteIndented = true });
//Console.WriteLine(message2);

//List<Person> _people = JsonSerializer.Deserialize<List<Person>>(message2) ?? new List<Person>();

// Deserialize a Person.
//Person temp = JsonSerializer.Deserialize<Person>(message) ?? new Person("Seoin", 4);
//Console.WriteLine("h");

Person person = new Student("Jim", (float)4.0, 5);
string serializedPerson = JsonSerializer.Serialize(person);
Console.WriteLine(serializedPerson);