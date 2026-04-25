using AvitoAppleParser.Db.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvitoAppleParser.Db
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Product> Products { get; set; }

        public DatabaseContext(DbContextOptions<DatabaseContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.ExternalId)
                .IsUnique();

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Price);

            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Title);
        }
    }
}
