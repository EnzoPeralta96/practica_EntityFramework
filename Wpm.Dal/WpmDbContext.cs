using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Wpm.Domain;

namespace Wpm.Dal;

public class WpmDbContext : DbContext
{
    private readonly string? connetionString;
    public DbSet<Breed> Breeds {get; set;}
    public DbSet<Pet> Pets {get; set;}
    public DbSet<Species> Species {get; set;}
    public DbSet<Owner> Owners {get; set;}
    public WpmDbContext(IConfiguration configuration) 
    {
        connetionString = configuration.GetConnectionString("SqliteConnection");
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(connetionString);
    }

}
