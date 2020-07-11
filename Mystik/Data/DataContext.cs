using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Mystik.Entities;

namespace Mystik.Data
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Conversation> Conversations { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
