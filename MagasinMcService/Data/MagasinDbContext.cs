using MagasinMcService.Models;
using Microsoft.EntityFrameworkCore;

namespace MagasinMcService.Data
{
    public class MagasinDbContext : DbContext
    {
        public MagasinDbContext(DbContextOptions<MagasinDbContext> options)
            : base(options)
        {
        }

        public DbSet<Magasin> Magasins { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Magasin>().HasData(
                new Magasin { MagasinId = 1, Nom = "Entrepot Central", Adresse = "123 Rue entrepot" },
                new Magasin { MagasinId = 2, Nom = "Magasin A", Adresse = "123 Rue Principale" },
                new Magasin { MagasinId = 3, Nom = "Magasin B", Adresse = "456 Avenue Centrale" },
                new Magasin { MagasinId = 4, Nom = "Magasin C", Adresse = "789 Boulevard Sud" },
                new Magasin { MagasinId = 5, Nom = "Magasin D", Adresse = "321 Rue Nord" }
            );
        }
    }
}
