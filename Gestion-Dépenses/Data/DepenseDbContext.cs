using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using Gestion_Dépenses.Models.UserModel;
using Gestion_Dépenses.Models.DepenseModel;

namespace Gestion_Dépenses.Data
{
    //classe de base pour configurer et utiliser ASP.NET Core Identity avec Entity Framework
    public class DepenseDbContext : IdentityDbContext<User, IdentityRole, string>
    {
        public DbSet<Depense> Depenses { get; set; }
        public DbSet<Deplacement> Deplacements { get; set; }
        public DbSet<Restaurant> Restaurants { get; set; }

        public DepenseDbContext(DbContextOptions<DepenseDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Depense>()
       .HasDiscriminator<TypeDepense>("Nature") 
       .HasValue<Deplacement>(TypeDepense.Deplacement)
       .HasValue<Restaurant>(TypeDepense.Restaurant);            
            modelBuilder.Entity<Depense>()
                .Property(d => d.Commentaire)
                .HasMaxLength(100);

            modelBuilder.Entity<Depense>()
                .Property(d => d.Montant)
                .HasPrecision(18, 2);

            modelBuilder.Entity<Deplacement>()
                .Property(d => d.Distance)
                .IsRequired();

            modelBuilder.Entity<Restaurant>()
                .Property(d => d.NombreInvites)
                .IsRequired();

            base.OnModelCreating(modelBuilder);
        }
    }
}

