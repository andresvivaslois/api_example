using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace GNB.Infrastructure.Models
{
    public class BankContext : DbContext
    {
        public DbSet<Rates> Rates { get; set; }
        public DbSet<Transactions> Transactions { get; set; }

        public BankContext(DbContextOptions<BankContext> options) : base(options) { }

        public BankContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data source = App_Data/bankDB.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Rates>()
                .Property(e => e.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Transactions>()
               .Property(e => e.Id)
               .ValueGeneratedOnAdd();
        }
    }
}
