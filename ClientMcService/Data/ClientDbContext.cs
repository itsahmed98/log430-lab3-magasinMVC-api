using ClientMcService.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientMcService.Data
{
    public class ClientDbContext : DbContext
    {
        public ClientDbContext(DbContextOptions<ClientDbContext> options) : base(options) { }

        public DbSet<Client> Clients { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Client>().HasData(
                new Client { ClientId = 1, Nom = "Alice Dupont", Courriel = "alice@dupont.ca", Adresse = "" },
                new Client { ClientId = 2, Nom = "Alex Alexandre", Courriel = "alex@alexandre.ca", Adresse = "" },
                new Client { ClientId = 3, Nom = "Chris Christopher", Courriel = "chris@christopher.ca", Adresse = "" },
                new Client { ClientId = 4, Nom = "Simon Samuel", Courriel = "simon@samuel.ca", Adresse = "" }
            );
        }
    }
}