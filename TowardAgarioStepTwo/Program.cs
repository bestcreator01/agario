// See https://aka.ms/new-console-template for more information
using System.Text.Json;
using TowardAgarioStepTwo;

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