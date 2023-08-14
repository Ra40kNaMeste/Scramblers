using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PassManager.Model
{
    public class PassDBContext : DbContext
    {
        public PassDBContext(string path)
        {
            _connectionString = $"Data Source={path}; Version=3;";
        }

        private readonly string _connectionString;

        public DbSet<Password> Passwords => Set<Password>();
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Password>().HasKey(x => x.Id);
            modelBuilder.Entity<Password>().Property(i => i.Name).HasDefaultValue("Site").IsRequired();
            modelBuilder.Entity<Password>().Property(i=>i.Value).IsRequired();
            base.OnModelCreating(modelBuilder);
        }
    }

    public class Password
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte[] Value { get; set; }
    }
}

