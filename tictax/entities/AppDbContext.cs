using entities.Configuration;
using entities.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace entities
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            //modelBuilder
            //    .Entity<User>()
            //    .HasMany(e => e.InitiatorProfileActivites)
            //    .WithOne(e => e.Initiator)
            //    .OnDelete(DeleteBehavior.ClientCascade);

            //modelBuilder
            //    .Entity<User>()
            //    .HasMany(e => e.RecipientProfileActivites)
            //    .WithOne(e => e.Recipient)
            //    .OnDelete(DeleteBehavior.ClientCascade);

            //modelBuilder.Entity<ProfileActivity>()
            //    .HasOne(p => p.Recipient)
            //    .WithMany(p => p.RecipientProfileActivites)
            //    .HasForeignKey(p => p.UsernameTo)
            //    .OnDelete(DeleteBehavior.ClientCascade);

            //modelBuilder.Entity<ProfileActivity>()
            //    .HasOne(p => p.Initiator)
            //    .WithMany(p => p.InitiatorProfileActivites)
            //    .HasForeignKey(p => p.UsernameFrom)
            //    .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.ApplyConfiguration(new UserConfiguration());   

            //modelBuilder
            //    .Entity<ProfileActivity>()
            //    .HasOne(e => e.Initiator)
            //    .WithMany(e => e.InitiatorProfileActivites)
            //    .OnDelete(DeleteBehavior.ClientCascade);

            //modelBuilder
            //    .Entity<ProfileActivity>()
            //    .HasOne(e => e.Recipient)
            //    .WithMany(e => e.RecipientProfileActivites)
            //    .OnDelete(DeleteBehavior.ClientCascade);

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<ProfileActivity> ProfileActivities { get; set; }

    }
}
