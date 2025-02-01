
using System.Net;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SQLitePCL;
using Wpm.Dal.Semilla;
using Wpm.Domain;

var builder = new ConfigurationBuilder();
builder.AddJsonFile("appsettings.json");
var config = builder.Build();

Batteries.Init();

using var db = new WpmDbContext2(config);
//db.Database.EnsureCreated();

//Eq a SELECT * FROM Species;
var species = db.Species.ToList();

foreach (var speciesX in species)
{
    Console.WriteLine("Especie:{0}",speciesX.Name);
}

var species2 = db.Species
                .Where(sp => sp.Id == 2)
                .ToList();

foreach (var speciesX in species2)
{
    Console.WriteLine("Especie:{0}",speciesX.Name);
}

var species3 = db.Species.FirstOrDefault(sp => sp.Id == 3);

Console.WriteLine("Especie:{0}",species3.Name);


/*
//Traer datos relacionados:
Obs: por defecto EF no trae los datos de campos
relacionados de forma automatica.
Con el metodo "include" podemos pasar una expre lambda indicando
cual es la propiedad que representa a la tabla relacionada.
*/

Console.WriteLine();

var breeds = db.Breeds
                .Include(br => br.Species)
                .ToList();//Para poder enviar la sentencia al server db

foreach (var item in breeds)
{
    Console.WriteLine("Raza:{0} - Espececie:{1}",item.Name, item.Species.Name);
}

//Traer las mascotas y sus dueños
Console.WriteLine();
var pets = db.Pets
            .Include(p => p.Owners)
            .ToList();

Console.WriteLine();
foreach (var item in pets)
{
    Console.WriteLine("Mascota:{0}", item.Name);
    foreach (var owner in item.Owners)
    {
        Console.WriteLine("\tDueño:{0}",owner.Name);
    }
    Console.WriteLine();
}

//Proyectar datos:
//Traer la especie y la raza
var breeds2 = db.Breeds
                .Include(br => br.Species)
                .Select(br => new {BreedName = br.Name, SpeciesName = br.Species.Name})//Genero un objeto dinamico 
                .ToList();
//En otro caso puedo usar un ViewModel en vez de generar un objeto dinámico

Console.WriteLine();
foreach (var item in breeds2)
{
    Console.WriteLine("Raza:{0} - Espececie:{1}",item.BreedName, item.SpeciesName);
}

//Ordenar resultado
//Este ordenamiento se hace por lado del servidor
var breeds3 = db.Breeds
                .Include(br => br.Species)
                .Select(br => new {BreedName = br.Name, SpeciesName = br.Species.Name})
                .OrderBy(br => br.SpeciesName)
                .ThenBy(br => br.BreedName)
                .ToList();

Console.WriteLine();
foreach (var item in breeds3)
{
    Console.WriteLine("Raza:{0} - Espececie:{1}",item.BreedName, item.SpeciesName);
}

/*
Observar que con ToList() se ejecuta la consulta hasta ese punto del lado del server
Una vez que tenemos los datos en memoria, recién se ordenan en el cliente. Ineficiente, 
aunque el resultado final será el mismo.
*/
var breeds4 = db.Breeds
                .Include(br => br.Species)
                .Select(br => new {BreedName = br.Name, SpeciesName = br.Species.Name})
                .ToList()
                .OrderBy(br => br.SpeciesName)
                .ThenBy(br => br.BreedName);

//Ejecución de consultas SQL:

/*
FromSqlRaw sirve para hacer consultas dinamicas, es decir que,
por ej el nombre de la columna a consultar en el WHERE sea una variable.
Y se deberá hacer los controles de seguridad necesarios para esa columna
y que no inyecten codigo sql.
Por otro lado, los valores de los la columna se envian encapsulados en DbParameter.
Ejemplo:
var columnName = "Url"; // Si este dato viene por UI, es el que hay que controlar para
que no venga con inyección SQL.
var columnValue = new SqlParameter("columnValue", "http://SomeURL");

var blogs = await context.Blogs
    .FromSqlRaw($"SELECT * FROM [Blogs] WHERE {columnName} = @columnValue", columnValue)
    .ToListAsync();

*/
var pets2 = db.Pets.FromSqlRaw("SELECT * FROM pets WHERE age > 20").ToList();

Console.WriteLine("Mascotas con mas de 20 años");
foreach (var item in pets2)
{
    Console.WriteLine(item.Name);
}

/*
Esta nos sirve para que cualquier valor que enviemos
en la consulta, no se concatene como un string, si no que 
se mande como un valor al db, encapsulado en un DbParameter.
De esta forma evitamos la inyección SQL.
*/

/*
Aquí parece que enviamos "age" como un valor, pero lo FromSqlIterpolated lo mando
como en dbParameters.
*/
int age = 10;
var pets3 = db.Pets.FromSqlInterpolated($"SELECT * FROM pets WHERE age < {age}");

Console.WriteLine($"Mascotas con menores de {age} años");
foreach (var item in pets3)
{
    Console.WriteLine(item.Name);
}

/*
Ejecutar store procedures:
- Que devuelve un resultado:
En este caso es sin parametros, por lo que podemos usar:
    var pets = db.Pets.FromSqlRaw("GetOverweightPets").ToList();

- Que no devuelve un resultado:
Como no devuelve un resultado,no esta relacionado sobre un DbSet,
por lo que tenemos que usar la propiedad DataBase del dbContext:
ExecuteSqlInterpolated() -> porque enviamos un parámetro

var filasAfectadas = db.Database.ExecuteSqlInterpolated($"DeleteOwner {7}");

*/