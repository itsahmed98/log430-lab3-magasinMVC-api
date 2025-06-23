using Microsoft.EntityFrameworkCore;
using PanierMcService.Models;

namespace PanierMcService.Data
{
    /// <summary>
    /// Contexte EF Core pour la gestion du panier.
    /// </summary>
    public class PanierDbContext : DbContext
    {
        public PanierDbContext(DbContextOptions<PanierDbContext> options)
            : base(options)
        {
        }

        public DbSet<Panier> Paniers { get; set; } = null!;
        public DbSet<LignePanier> LignesPanier { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Relation : un panier a plusieurs lignes
            modelBuilder.Entity<LignePanier>()
                .HasOne<Panier>()
                .WithMany(p => p.Lignes)
                .HasForeignKey(lp => lp.PanierId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed optionnel pour les tests
            modelBuilder.Entity<Panier>().HasData(
                new Panier { PanierId = 1, ClientId = 2 }
            );

            modelBuilder.Entity<LignePanier>().HasData(
                new LignePanier { LignePanierId = 1, PanierId = 1, ProduitId = 1, Quantite = 2 },
                new LignePanier { LignePanierId = 2, PanierId = 1, ProduitId = 2, Quantite = 1 }
            );
        }
    }
}
