using Microsoft.Extensions.Configuration;
using SQLitePCL;
using Wpm.Dal;


var builder = new ConfigurationBuilder();
builder.AddJsonFile("appsettings.json");
var config = builder.Build();

Batteries.Init();

using var db = new WpmDbContext(config);
db.Database.EnsureCreated();

