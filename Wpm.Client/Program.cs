using System.ComponentModel;
using Microsoft.Extensions.Configuration;
using SQLitePCL;
using Wpm.Dal;
using Wpm.Domain;


var builder = new ConfigurationBuilder();
builder.AddJsonFile("appsettings.json");
var config = builder.Build();

Batteries.Init();

using var db = new WpmDbContext(config);
db.Database.EnsureDeleted();
db.Database.EnsureCreated();

//Creo especies
var perro = new Species(){ Name = "Perro"};
var gato = new Species(){ Name = "Gato"};

//Agrego las especies al dbSet
db.Species.Add(perro);
db.Species.Add(gato);

db.SaveChanges();

//Creo razas
//Paso el objeto perro, para indicar la fk
var beagle = new Breed(){ Name = "Beagle", Species = perro};

//Pero tambien le puedo pasar el valor de la fk directamente
var pitbull = new Breed(){ Name = "Pitbull", SpeciesId = 1};
var siames = new Breed(){Name = "Siamés", SpeciesId = 2};

db.Breeds.Add(beagle);
db.Breeds.Add(pitbull);
db.Breeds.Add(siames);

db.SaveChanges();

//Agregando mascotas:
var gianni = new Pet(){ Name = "Gianni" , Breed = beagle};
var cati = new Pet(){ Name = "Cati", BreedId = 2};

db.Pets.Add(gianni);
db.Pets.Add(cati);

db.SaveChanges();

//Agregando dueños de las mascotas- relacion n:n
var rodrigo = new Owner(){Name = "Rodrigo"};
var leonardo = new Owner(){ Name = "Leonardo"};

//En este caso EF detecta la relación entre Pet y Owner
//Agrega los dueños a la tabla Owner y luego los relaciona
//Notar que no agregamos los dueños a su dbSet y aún asi funciono.
gianni.Owners.Add(rodrigo);
gianni.Owners.Add(leonardo);
cati.Owners.Add(rodrigo);
cati.Owners.Add(leonardo);

db.SaveChanges();


Console.WriteLine("Listo");

//Modifcando datos existentes.
cati.Age = 10;
cati.Weight = (float)19.3;

gianni.Age = 8;
gianni.Weight = (float)33.5;

db.SaveChanges();