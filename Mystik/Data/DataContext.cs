using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mystik.Entities;

namespace Mystik.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        protected readonly IConfiguration _configuration;

        public DataContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connectionString = _configuration["DEFAULT_CONNECTION"];
            optionsBuilder.UseNpgsql(connectionString);
        }
    }
}
