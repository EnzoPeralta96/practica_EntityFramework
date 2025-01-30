
using Microsoft.Extensions.Configuration;
using SQLitePCL;
using Wpm.Dal.Semilla;
using Wpm.Domain;

var builder = new ConfigurationBuilder();
builder.AddJsonFile("appsettings.json");
var config = builder.Build();

Batteries.Init();

using var db = new WpmDbContext2(config);
db.Database.EnsureCreated();

